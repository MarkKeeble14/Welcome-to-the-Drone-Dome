﻿using System.Collections;
using UnityEngine;

public abstract class DamageTriggerField : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected StatModifier damage;
    [SerializeField] private StatModifier tickSpeed;
    [SerializeField] private StatModifier duration;
    [SerializeField] private StatModifier growSpeed;
    private LayerMask enemyLayer;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();
    protected bool reachedMaxRadius;

    protected void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(duration.Value);

        StopAllCoroutines();

        StartCoroutine(Fade());
    }

    public void SetRadius(float radius)
    {
        StartCoroutine(Grow(radius));
    }


    private IEnumerator Grow(float radius)
    {
        while (transform.localScale.x != radius)
        {
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, Vector3.one * radius, growSpeed.Value * Time.deltaTime);

            yield return null;
        }

        reachedMaxRadius = true;
    }

    private IEnumerator Fade()
    {
        reachedMaxRadius = false;
        while (transform.localScale != Vector3.zero)
        {
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, Vector3.zero, growSpeed.Value * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, enemyLayer)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        HitHealthBehaviour(hb);
        sameTargetCDDictionary.Add(other.gameObject, tickSpeed.Value);
    }

    protected virtual void HitHealthBehaviour(HealthBehaviour hb)
    {
        hb.Damage(damage.Value, true);
    }

    protected void Update()
    {
        sameTargetCDDictionary.Update();
    }
}
