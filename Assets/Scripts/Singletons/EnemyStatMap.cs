using UnityEngine;

[System.Serializable]
public class EnemyStatMap
{
    [SerializeField] public GrowthStatModifier DamageMod;
    [SerializeField] public GrowthStatModifier MaxHealthMod;
    [SerializeField] public GrowthStatModifier SpeedMod;
    [SerializeField] public GrowthStatModifier NumEnemiesAliveMod;
    [SerializeField] public GrowthStatModifier NumEnemiesToKillMod;

    public void Grow()
    {
        SpeedMod.Grow();
        MaxHealthMod.Grow();
        DamageMod.Grow();
        NumEnemiesAliveMod.Grow();
        NumEnemiesToKillMod.Grow();
    }

    public void Reset()
    {
        SpeedMod.Reset();
        MaxHealthMod.Reset();
        DamageMod.Reset();
        NumEnemiesAliveMod.Reset();
        NumEnemiesToKillMod.Reset();
    }
}
