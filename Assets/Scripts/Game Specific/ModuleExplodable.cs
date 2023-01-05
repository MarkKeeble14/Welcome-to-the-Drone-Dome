using UnityEngine;

public class ModuleExplodable : StatModifierExplodable
{
    [SerializeField] private ModuleType source;

    public override void Explode()
    {
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position, source);
    }
}
