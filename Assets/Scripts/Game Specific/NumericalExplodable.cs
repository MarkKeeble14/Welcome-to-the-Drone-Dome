using UnityEngine;

public class NumericalExplodable : Explodable
{
    [SerializeField] private NumericalExplosionData explosionData;
    public override void Explode()
    {
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position);
    }
}

