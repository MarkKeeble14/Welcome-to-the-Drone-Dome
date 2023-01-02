using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField] protected ExplosionData explosionData;

    public bool AllowChainExplosion = true;

    private bool hasAlreadyExploded = false;

    public void CallExplode()
    {
        if (hasAlreadyExploded) return;
        hasAlreadyExploded = true;
        Explode();
        Destroy(gameObject);
    }

    protected virtual void Explode()
    {
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position);
    }
}
