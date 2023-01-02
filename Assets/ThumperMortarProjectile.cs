using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperMortarProjectile : MortarProjectile
{
    [SerializeField] private Thumper thumper;
    private bool activated;

    public override void ArrivedAtPosition()
    {
        if (activated) return;
        activated = true;
        thumper.Thump();
    }
}
