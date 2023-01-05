using UnityEngine;

public class StatModifierExplodable : Explodable
{
    [SerializeField] protected StatModifierExplosionData explosionData;
    public override void Explode()
    {
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position);
    }
}

