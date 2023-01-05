[System.Serializable]
public class EnemyStatMap
{
    public GrowthStatModifier DamageMod;
    public GrowthStatModifier MaxHealthMod;
    public GrowthStatModifier SpeedMod;
    public GrowthStatModifier NumEnemiesAliveMod;
    public GrowthStatModifier NumEnemiesToKillMod;

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
