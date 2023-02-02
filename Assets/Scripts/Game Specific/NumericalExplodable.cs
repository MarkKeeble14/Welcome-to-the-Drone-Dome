using UnityEngine;

public class NumericalExplodable : Explodable
{
    [SerializeField] private NumericalExplosionData explosionData;
    public override void Explode()
    {
        explosionData.Damage *= explosionDamageModifier;
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position);
    }
}

