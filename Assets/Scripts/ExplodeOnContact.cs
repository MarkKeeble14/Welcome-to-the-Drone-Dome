using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnContact : MonoBehaviour
{
    [SerializeField] private LayerMask explodeOnContactWith;
    [SerializeField] private Explodable explodable;

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnContactWith)) return;
        explodable.CallExplode(true);
        Destroy(gameObject);
    }
}
