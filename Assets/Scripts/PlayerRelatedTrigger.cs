using UnityEngine;

public class PlayerRelatedTrigger : MonoBehaviour
{
    protected LayerMask playerLayer;
    private void Awake()
    {
        playerLayer = LayerMask.GetMask("Player", "PlayerIgnoreEnemy");
    }
}
