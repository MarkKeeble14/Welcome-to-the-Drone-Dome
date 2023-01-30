using System;
using System.Collections;
using UnityEngine;

public abstract class DamageTriggerField : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected float damage;
    [SerializeField] private float tickSpeed;
    [SerializeField] private float duration;
    [SerializeField] private float growSpeed;
    private LayerMask enemyLayer;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();
    protected bool reachedMaxRadius;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip endClip;

    protected void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected void Update()
    {
        sameTargetCDDictionary.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, enemyLayer)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        HitHealthBehaviour(hb);
        sameTargetCDDictionary.Add(other.gameObject, tickSpeed);
    }

    public void Set(float radius, float damage, float tickSpeed, float duration, float growSpeed, Action onEnd)
    {
        this.damage = damage;
        this.tickSpeed = tickSpeed;
        this.duration = duration;
        this.growSpeed = growSpeed;
        foreach (Transform child in transform)
        {
            child.localPosition = Vector3.zero;
        }
        StartCoroutine(Lifetime(radius, onEnd));
    }

    private IEnumerator Lifetime(float radius, Action onEnd)
    {
        StartCoroutine(Grow(radius));

        yield return new WaitForSeconds(duration);

        StopAllCoroutines();

        StartCoroutine(Fade(onEnd));
    }


    private IEnumerator Grow(float radius)
    {
        // Audio
        sfxSource.PlayOneShot(startClip);

        while (transform.localScale.x != radius)
        {
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, Vector3.one * radius, growSpeed * Time.deltaTime);

            yield return null;
        }

        // Debug.Log("Reached Max Radius");
        reachedMaxRadius = true;
    }

    private IEnumerator Fade(Action onEnd)
    {
        reachedMaxRadius = false;
        while (transform.localScale != Vector3.zero)
        {
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, Vector3.zero, growSpeed * Time.deltaTime);

            yield return null;
        }

        // Audio
        sfxSource.PlayOneShot(endClip);

        onEnd?.Invoke();
    }

    protected virtual void HitHealthBehaviour(HealthBehaviour hb)
    {
        hb.Damage(damage, true);
    }
}
