using UnityEngine;

public class ModuleExplodable : Explodable
{
    [SerializeField] private ModuleType source;

    protected override void Explode()
    {
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position, source);
    }
}
