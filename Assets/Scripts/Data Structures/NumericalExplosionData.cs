using UnityEngine;

[System.Serializable]
public class NumericalExplosionData : ExplosionData
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float power = 500f;
    [SerializeField] private float lift = 50f;
    [SerializeField] private float damage = 10f;
    public override float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public override float Power
    {
        get { return power; }
        set { power = value; }
    }

    public override float Lift
    {
        get { return lift; }
        set { lift = value; }
    }

    public override float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
}
