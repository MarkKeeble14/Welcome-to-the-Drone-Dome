using System.Collections.Generic;
using UnityEngine;

public class DroneThumperModule : DroneMortarModule
{
    [Header("Thumper Stats")]
    [SerializeField] private LoadStatModifierInfo numThumps;
    [SerializeField] private LoadStatModifierInfo timeBetweenThumps;
    [SerializeField] private LoadStatModifierInfo radius;
    [SerializeField] private LoadStatModifierInfo expansionSpeed;
    [SerializeField] private LoadStatModifierInfo damage;
    [SerializeField] private LoadStatModifierInfo knockback;
    public override ModuleType Type => ModuleType.SHOCKWAVE_MORTAR;

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        numThumps.SetStat(UpgradeNode.GetStatModifierUpgradeNode(numThumps, allModuleUpgradeNodes));
        timeBetweenThumps.SetStat(UpgradeNode.GetStatModifierUpgradeNode(timeBetweenThumps, allModuleUpgradeNodes));
        radius.SetStat(UpgradeNode.GetStatModifierUpgradeNode(radius, allModuleUpgradeNodes));
        expansionSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(expansionSpeed, allModuleUpgradeNodes));
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
        knockback.SetStat(UpgradeNode.GetStatModifierUpgradeNode(knockback, allModuleUpgradeNodes));
    }

    protected override void SetWeaponSpecificProjectileInfo(Projectile proj)
    {
        ThumperMortarProjectile thumpProj = (ThumperMortarProjectile)proj;
        thumpProj.SetStats((int)numThumps.Stat.Value, timeBetweenThumps.Stat.Value, radius.Stat.Value,
            expansionSpeed.Stat.Value, damage.Stat.Value, knockback.Stat.Value);
    }
}
