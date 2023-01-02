using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveMortarProjectile : MortarProjectile
{
    [SerializeField] private Explodable explodable;

    public override void ArrivedAtPosition()
    {
        explodable.CallExplode();
    }
}
