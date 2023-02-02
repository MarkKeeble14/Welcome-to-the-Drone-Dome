using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingShoveable : Shoveable
{
    [SerializeField] private LayerMask explodeOnCollideWith;
    [SerializeField] private Explodable explodable;


    private void OnCollisionStay(Collision collision)
    {
        if (!primed) return;
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnCollideWith)) return;
        explodable.SetExplosionDamageModifier(MathHelper.Normalize(rb.velocity.magnitude, .25f, 25f, .75f, 3f));
        explodable.CallExplode(true);
    }


    private float GetXandZVelocityMagnitude()
    {
        return Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));
    }
}
