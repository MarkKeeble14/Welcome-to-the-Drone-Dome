using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFieldMortarProjectile : MortarProjectile
{
    [SerializeField] private DamageTriggerField poisonField;
    [SerializeField] private float radius;

    public override void ArrivedAtPosition()
    {
        DamageTriggerField spawned = Instantiate(poisonField, transform.position, Quaternion.identity);
        spawned.SetRadius(radius);
        Destroy(gameObject);
    }
}
