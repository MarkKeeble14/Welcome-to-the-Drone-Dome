using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFiringRangeUITrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if colliding with Player
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        FiringRangeManager._Instance.Open();
    }
}