using UnityEngine;

[System.Serializable]
public class StatModifierExplosionData : ExplosionData
{
    [SerializeField] private StatModifier radius;
    [SerializeField] private StatModifier power;
    [SerializeField] private StatModifier lift;
    [SerializeField] private StatModifier damage;

    public override float Radius => radius.Value;

    public override float Power => power.Value;

    public override float Lift => lift.Value;

    public override float Damage => damage.Value;
}
