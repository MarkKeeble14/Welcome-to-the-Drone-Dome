using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDronesTrigger : PlayerRelatedTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if colliding with Player
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, playerLayer)) return;
        FiringRangeManager._Instance.ResetFiringRange();
    }
}
