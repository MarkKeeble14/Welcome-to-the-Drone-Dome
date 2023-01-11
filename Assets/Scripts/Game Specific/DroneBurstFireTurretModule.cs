using UnityEngine;
using UnityEngine.Pool;

public class DroneBurstFireTurretModule : BulletTypeTurretModule
{
    [Header("Burst Fire")]
    [SerializeField] private LoadStatModifierInfo projectilesInBurst;
    [SerializeField] private float timeBetweenBullets = .05f;
    public override ModuleType Type => ModuleType.BURST_FIRE_TURRET;

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        projectilesInBurst.SetStat(UpgradeNode.GetStatModifierUpgradeNode(projectilesInBurst, allModuleUpgradeNodes));
    }

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        BurstFireGunHelper._Instance.CallBurstFire(
            () => base.Shoot(projectileOrigin, shootAt, source),
            timeBetweenBullets,
            projectilesInBurst.Stat.Value);
        return (1 / shotsPerSecond.Stat.Value);
    }
}
