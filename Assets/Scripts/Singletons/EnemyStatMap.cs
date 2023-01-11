using UnityEngine;

[System.Serializable]
public class EnemyStatMap
{
    [SerializeField] public GrowthStatModifier1 DamageMod;
    [SerializeField] public GrowthStatModifier1 MaxHealthMod;
    [SerializeField] public GrowthStatModifier1 SpeedMod;
    [SerializeField] public GrowthStatModifier1 NumEnemiesAliveMod;
    [SerializeField] public GrowthStatModifier1 NumEnemiesToKillMod;

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
