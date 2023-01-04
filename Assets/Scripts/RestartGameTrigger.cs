using UnityEngine;

public class RestartGameTrigger : MonoBehaviour
{
    private bool triggered;
    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        if (triggered) return;
        triggered = true;
        GameManager._Instance.RestartGame();
    }
}
