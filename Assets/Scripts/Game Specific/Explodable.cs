using UnityEngine;

public abstract class Explodable : MonoBehaviour, IExplodable
{
    public bool AllowChainExplosion = true;
    private bool hasAlreadyExploded;

    private void OnEnable()
    {
        hasAlreadyExploded = false;
    }

    public void CallExplode(bool destroy)
    {
        if (hasAlreadyExploded) return;
        hasAlreadyExploded = true;
        Explode();
        if (destroy)
            Destroy(gameObject);
    }

    public abstract void Explode();
}

