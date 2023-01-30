using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginGameTrigger : PlayerRelatedTrigger
{
    private bool triggered;
    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, playerLayer)) return;
        if (triggered) return;
        triggered = true;
        GameManager._Instance.StartGame();
    }
}
