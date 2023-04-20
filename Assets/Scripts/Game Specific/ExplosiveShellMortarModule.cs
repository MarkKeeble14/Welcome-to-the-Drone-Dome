using UnityEngine;

public class ExplosiveShellMortarModule : DroneMortarModule
{
    [Header("Explosive Shell Dropper")]
    [SerializeField] private LoadStatModifierInfo mainDamage;
    [SerializeField] private LoadStatModifierInfo mainRadius;
    [SerializeField] private LoadStatModifierInfo mainPower;
    [SerializeField] private StatModifier mainLift;
    [SerializeField] private LoadBoolSwitchInfo shouldDrop;
    [SerializeField] private LoadStatModifierInfo timeBetweenDrops;
    [SerializeField] private LoadStatModifierInfo miniDamage;
    [SerializeField] private LoadStatModifierInfo miniRadius;
    [SerializeField] private LoadStatModifierInfo miniPower;
    [SerializeField] private LoadBoolSwitchInfo targetFurthest;
    [SerializeField] private StatModifier miniLift;

    protected override WeaponTargetingType TargetBy => targetFurthest.BoolSwitch.Active ? WeaponTargetingType.FURTHEST : WeaponTargetingType.CLOSEST;

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

        targetFurthest.SetBoolSwitch(UpgradeNode.GetBoolSwitchUpgradeNode(targetFurthest, allModuleUpgradeNodes));
    }

    protected override void SetWeaponSpecificProjectileInfo(Projectile proj)
    {
        ExplosiveMortarProjectile exDropperProj = ((ExplosiveMortarProjectile)proj);
        exDropperProj.Set(mainDamage.Stat, mainRadius.Stat, mainPower.Stat, mainLift,
            shouldDrop.BoolSwitch, timeBetweenDrops.Stat, miniDamage.Stat, miniRadius.Stat, miniPower.Stat, miniLift);
    }
}

