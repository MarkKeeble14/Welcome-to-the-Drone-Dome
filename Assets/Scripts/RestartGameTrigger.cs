using UnityEngine;

public class RestartGameTrigger : PlayerRelatedTrigger
{
    private bool triggered;
    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, playerLayer)) return;
        if (triggered) return;
        triggered = true;
        GameManager._Instance.RestartGame();
    }
}
