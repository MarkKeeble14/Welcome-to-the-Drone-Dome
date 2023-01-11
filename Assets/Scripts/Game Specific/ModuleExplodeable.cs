using UnityEngine;

public class ModuleExplodeable : Explodable
{
    [SerializeField] protected NumericalExplosionData explosionData;
    [SerializeField] private ModuleType source;

    public override void Explode()
    {
        ExplosionHelper.ExplodeEnemiesAt(explosionData, transform.position, source);
    }

    public void SetExplosionData(float damage, float radius, float power, float lift)
    {
        explosionData.Damage = damage;
        explosionData.Radius = radius;
        explosionData.Power = power;
        explosionData.Lift = lift;
    }
}

