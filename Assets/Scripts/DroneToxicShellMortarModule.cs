using UnityEngine;

public class DroneToxicShellMortarModule : DroneMortarModule
{
    [SerializeField] private LoadStatModifierInfo tickDamage;
    [SerializeField] private LoadStatModifierInfo tickSpeed;
    [SerializeField] private LoadStatModifierInfo duration;
    [SerializeField] private LoadStatModifierInfo radius;
    [SerializeField] private LoadStatModifierInfo initialExpansionSpeed;
    [SerializeField] private LoadBoolSwitchInfo shouldExpand;
    [SerializeField] private LoadStatModifierInfo expandSpeed;

    public override ModuleType Type => ModuleType.TOXIC_SHELL_MORTAR;

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        tickDamage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(tickDamage, allModuleUpgradeNodes));
        tickSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(tickSpeed, allModuleUpgradeNodes));
        duration.SetStat(UpgradeNode.GetStatModifierUpgradeNode(duration, allModuleUpgradeNodes));
        radius.SetStat(UpgradeNode.GetStatModifierUpgradeNode(radius, allModuleUpgradeNodes));
        initialExpansionSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(initialExpansionSpeed, allModuleUpgradeNodes));
        expandSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(expandSpeed, allModuleUpgradeNodes));
        shouldExpand.SetBoolSwitch(UpgradeNode.GetBoolSwitchUpgradeNode(shouldExpand, allModuleUpgradeNodes));
    }


    protected override void SetWeaponSpecificProjectileInfo(Projectile proj)
    {
        ToxicFieldMortarProjectile toxicShellProj = (ToxicFieldMortarProjectile)proj;
        toxicShellProj.SetStats(radius.Stat.Value, tickDamage.Stat.Value, tickSpeed.Stat.Value, duration.Stat.Value,
            initialExpansionSpeed.Stat.Value, shouldExpand.BoolSwitch.Active, expandSpeed.Stat.Value);
    }
}
