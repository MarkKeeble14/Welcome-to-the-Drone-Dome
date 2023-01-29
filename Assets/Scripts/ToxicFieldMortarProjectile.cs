using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicFieldMortarProjectile : MortarProjectile
{
    [SerializeField] private DamageTriggerFieldType fieldType = DamageTriggerFieldType.TOXIC_FIELD;
    [SerializeField] private float radius;
    [SerializeField] protected float damage;
    [SerializeField] private float tickSpeed;
    [SerializeField] private float duration;
    [SerializeField] private float growSpeed;
    [SerializeField] private bool shouldExpand;
    [SerializeField] private float expandSpeed;

    public void SetStats(float radius, float damage, float tickSpeed, float duration, float growSpeed,
        bool shouldExpand, float expandSpeed)
    {
        this.radius = radius;
        this.damage = damage;
        this.tickSpeed = tickSpeed;
        this.duration = duration;
        this.growSpeed = growSpeed;
        this.shouldExpand = shouldExpand;
        this.expandSpeed = expandSpeed;
    }

    public override void ArrivedAtPosition()
    {
        // Spawn
        ToxicFieldTriggerField spawned = (ToxicFieldTriggerField)ObjectPooler._Instance.GetDamageTriggerField(fieldType);
        spawned.transform.position = transform.position;

        // Set
        spawned.Set(shouldExpand, expandSpeed);
        spawned.Set(radius, damage, tickSpeed, duration, growSpeed,
            () => ObjectPooler._Instance.ReleaseDamageTriggerField(fieldType, spawned));

        // Release
        ReleaseAction?.Invoke();
    }
}
