using UnityEngine;

public class ExplosiveShellMortarModule : DroneMortarModule
{
    [Header("Explosive Shell Dropper")]
    [SerializeField] private LoadStatModifierInfo mainDamage;
    [SerializeField] private LoadStatModifierInfo mainRadius;
    [SerializeField] private LoadStatModifierInfo mainPower;
    [SerializeField] private StatModifier1 mainLift;
    [SerializeField] private LoadBoolSwitchInfo shouldDrop;
    [SerializeField] private LoadStatModifierInfo timeBetweenDrops;
    [SerializeField] private LoadStatModifierInfo miniDamage;
    [SerializeField] private LoadStatModifierInfo miniRadius;
    [SerializeField] private LoadStatModifierInfo miniPower;
    [SerializeField] private StatModifier1 miniLift;
    public override ModuleType Type => ModuleType.EXPLOSIVE_SHELL_MORTAR;

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        mainDamage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(mainDamage, allModuleUpgradeNodes));
        mainRadius.SetStat(UpgradeNode.GetStatModifierUpgradeNode(mainRadius, allModuleUpgradeNodes));
        mainPower.SetStat(UpgradeNode.GetStatModifierUpgradeNode(mainPower, allModuleUpgradeNodes));

        miniDamage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(miniDamage, allModuleUpgradeNodes));
        miniRadius.SetStat(UpgradeNode.GetStatModifierUpgradeNode(miniRadius, allModuleUpgradeNodes));
        miniPower.SetStat(UpgradeNode.GetStatModifierUpgradeNode(miniPower, allModuleUpgradeNodes));

        timeBetweenDrops.SetStat(UpgradeNode.GetStatModifierUpgradeNode(timeBetweenDrops, allModuleUpgradeNodes));
        shouldDrop.SetBoolSwitch(UpgradeNode.GetBoolSwitchUpgradeNode(shouldDrop, allModuleUpgradeNodes));
    }

    protected override void SetWeaponSpecificProjectileInfo(Projectile proj)
    {
        ExplosiveMortarProjectile exDropperProj = ((ExplosiveMortarProjectile)proj);
        exDropperProj.Set(mainDamage.Stat, mainRadius.Stat, mainPower.Stat, mainLift,
            shouldDrop.BoolSwitch, miniDamage.Stat, miniRadius.Stat, miniPower.Stat, miniLift, timeBetweenDrops.Stat);
    }
}

