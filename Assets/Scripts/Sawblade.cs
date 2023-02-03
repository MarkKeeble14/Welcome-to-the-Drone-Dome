using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : Projectile
{
    private LayerMask enemyLayer;
    [SerializeField] private Rigidbody rb;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();

    private float damage;
    private float tickSpeed;
    private float speed;
    private Vector3 direction;

    [Header("Audio")]
    [SerializeField] private AudioSource whirringSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitClip;


    private void Start()
    {
        whirringSource.pitch = RandomHelper.RandomFloat(.9f, 1.1f);
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected void Update()
    {
        sameTargetCDDictionary.Update();
    }

    private void FixedUpdate()
    {
        if (direction != Vector3.zero)
        {
            rb.velocity = (speed * Time.deltaTime) * direction;
        }
    }

    protected virtual void HitHealthBehaviour(HealthBehaviour hb)
    {
        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(hitClip, .75f);

        hb.Damage(damage, ModuleType.SAWBLADE_ACTIVE);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, enemyLayer)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        HitHealthBehaviour(hb);
        sameTargetCDDictionary.Add(other.gameObject, tickSpeed);
    }

    public void Set(float damage, float tickSpeed, float size, float range, float speed, Vector3 direction)
    {
        // Reset
        sameTargetCDDictionary.Clear();

        // Set
        this.speed = speed;
        this.direction = direction;
        this.damage = damage;
        this.tickSpeed = tickSpeed;
        transform.localScale = Vector3.one * size;
        StartCoroutine(Lifetime(range));
    }

    private IEnumerator Lifetime(float range)
    {
        Vector3 startPos = transform.position;

        yield return new WaitUntil(() => Vector3.Distance(startPos, transform.position) > range);

        ReleaseAction();
    }
}
