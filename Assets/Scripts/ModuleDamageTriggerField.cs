public abstract class ModuleDamageTriggerField : DamageTriggerField
{
    public abstract ModuleType Source { get; }

    protected override void HitHealthBehaviour(HealthBehaviour hb)
    {
        hb.Damage(damage.Value, Source);
    }
}
