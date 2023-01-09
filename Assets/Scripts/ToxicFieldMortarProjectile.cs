using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicFieldMortarProjectile : MortarProjectile
{
    [SerializeField] private DamageTriggerFieldType fieldType = DamageTriggerFieldType.TOXIC_FIELD;
    [SerializeField] private StatModifier radius;

    public override void ArrivedAtPosition()
    {
        DamageTriggerField spawned = ObjectPooler._Instance.GetDamageTriggerField(fieldType);
        spawned.transform.position = transform.position;
        spawned.Set(radius.Value, () => ObjectPooler._Instance.ReleaseDamageTriggerField(fieldType, spawned));
        ReleaseAction?.Invoke();
    }
}
