using UnityEngine;

[System.Serializable]
public class NumericalExplosionData : ExplosionData
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float power = 500f;
    [SerializeField] private float lift = 50f;
    [SerializeField] private float damage = 10f;
    public override float Radius => radius;

    public override float Power => power;

    public override float Lift => lift;

    public override float Damage => damage;
}
