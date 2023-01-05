using UnityEngine;

public abstract class Explodable : MonoBehaviour, IExplodable
{
    public bool AllowChainExplosion = true;

    private bool hasAlreadyExploded = false;

    public void CallExplode()
    {
        if (hasAlreadyExploded) return;
        hasAlreadyExploded = true;
        Explode();
        Destroy(gameObject);
    }

    public abstract void Explode();
}

