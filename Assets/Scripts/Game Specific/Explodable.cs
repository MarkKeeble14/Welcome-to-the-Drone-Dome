using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField] private ExplosionData explosionData;

    public bool AllowChainExplosion = true;

    private bool hasAlreadyExploded = false;

    public void Explode()
    {
        if (hasAlreadyExploded) return;
        hasAlreadyExploded = true;
        ExplosionHelper.ExplodeAt(explosionData, transform.position);
        Destroy(gameObject);
    }
}
