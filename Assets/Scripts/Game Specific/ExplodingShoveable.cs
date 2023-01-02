using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingShoveable : Shoveable
{
    [SerializeField] private LayerMask explodeOnCollideWith;
    [SerializeField] private float activationThreshold;
    [SerializeField] private Explodable explodable;

    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (GetXandZVelocityMagnitude() < activationThreshold)
        {
            Primed = false;
        }
        */
        if (!Primed) return;
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnCollideWith)) return;
        explodable.CallExplode();
    }


    private float GetXandZVelocityMagnitude()
    {
        return Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));
    }
}
