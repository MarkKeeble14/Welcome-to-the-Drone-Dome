using UnityEngine;

public abstract class BulletTypeTurretModule : DroneGunModule
{
    [Header("Bullet Type")]
    [SerializeField] protected LoadStatModifierInfo projectileDamage;
    [SerializeField] protected LoadStatModifierInfo projectileBounce;
    [SerializeField] private LoadBoolSwitchInfo smartBounce;
    [SerializeField] protected LoadStatModifierInfo projectilePierce;
    [SerializeField] protected LoadStatModifierInfo projectilesPerShot;
    [SerializeField] private LoadStatModifierInfo angleBetweenProjectiles;

    [Header("Stats")]
    [SerializeField] protected float shootForce = 250;

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        projectilesPerShot.SetStat(UpgradeNode.GetStatModifierUpgradeNode(projectilesPerShot, allModuleUpgradeNodes));
        angleBetweenProjectiles.SetStat(UpgradeNode.GetStatModifierUpgradeNode(angleBetweenProjectiles, allModuleUpgradeNodes));
        projectileDamage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(projectileDamage, allModuleUpgradeNodes));
        projectileBounce.SetStat(UpgradeNode.GetStatModifierUpgradeNode(projectileBounce, allModuleUpgradeNodes));
        smartBounce.SetBoolSwitch(UpgradeNode.GetBoolSwitchUpgradeNode(smartBounce, allModuleUpgradeNodes));
        projectilePierce.SetStat(UpgradeNode.GetStatModifierUpgradeNode(projectilePierce, allModuleUpgradeNodes));

    }

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        // If shootAt is null, it means that whatever we were planning on shooting is no longer with us
        // Return 0 as the gun cd, so system can re-target and re-fire
        if (shootAt == null) return 0;

        if (Mathf.Floor(projectilesPerShot.Stat.Value) == 1)
        {
            ShootOne(projectileOrigin, shootAt, source);
        }
        else if (Mathf.Floor(projectilesPerShot.Stat.Value) % 2 == 0)
        {
            ShootEven(projectileOrigin, shootAt, source);
        }
        else if (Mathf.Floor(projectilesPerShot.Stat.Value) % 2 == 1)
        {
            ShootOdd(projectileOrigin, shootAt, source);
        }

        return base.Shoot(projectileOrigin, shootAt, source);
    }

    private void ShootOne(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        BulletTypeProjectile currentProjectile = (BulletTypeProjectile)ObjectPooler._Instance.GetProjectile(source);
        currentProjectile.ReleaseAction = () =>
        {
            ObjectPooler._Instance.ReleaseProjectile(source, currentProjectile);
        };
        projectileOrigin.y = GameManager._BaseHeight;
        currentProjectile.transform.position = projectileOrigin;

        Vector3 direction = shootAt.position - projectileOrigin;
        currentProjectile.Set(projectileDamage.Stat.Value,
            Mathf.RoundToInt(projectileBounce.Stat.Value),
            smartBounce.BoolSwitch.Active,
            Mathf.RoundToInt(projectilePierce.Stat.Value),
            direction.normalized, shootForce, source);
    }

    private void ShootEven(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        Vector3 direction = shootAt.position - projectileOrigin;
        direction = Quaternion.AngleAxis(((projectilesPerShot.Stat.Value / 2) - .5f) * -angleBetweenProjectiles.Stat.Value, Vector3.up) * direction;

        for (int i = 0; i < projectilesPerShot.Stat.Value; i++)
        {
            BulletTypeProjectile currentProjectile = (BulletTypeProjectile)ObjectPooler._Instance.GetProjectile(source);
            currentProjectile.ReleaseAction = () =>
            {
                ObjectPooler._Instance.ReleaseProjectile(source, currentProjectile);
            };
            projectileOrigin.y = GameManager._BaseHeight;
            currentProjectile.transform.position = projectileOrigin;

            currentProjectile.Set(projectileDamage.Stat.Value,
                Mathf.RoundToInt(projectileBounce.Stat.Value),
                smartBounce.BoolSwitch.Active,
                Mathf.RoundToInt(projectilePierce.Stat.Value),
                direction.normalized, shootForce, source);
            direction = Quaternion.AngleAxis(angleBetweenProjectiles.Stat.Value, Vector3.up) * direction;
        }
    }

    private void ShootOdd(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        Vector3 direction = shootAt.position - projectileOrigin;
        direction = Quaternion.AngleAxis(Mathf.Floor(projectilesPerShot.Stat.Value / 2) * -angleBetweenProjectiles.Stat.Value, Vector3.up) * direction;

        for (int i = 0; i < projectilesPerShot.Stat.Value; i++)
        {
            BulletTypeProjectile currentProjectile = (BulletTypeProjectile)ObjectPooler._Instance.GetProjectile(source);
            currentProjectile.ReleaseAction = () =>
            {
                ObjectPooler._Instance.ReleaseProjectile(source, currentProjectile);
            };
            projectileOrigin.y = GameManager._BaseHeight;
            currentProjectile.transform.position = projectileOrigin;

            currentProjectile.Set(projectileDamage.Stat.Value,
                Mathf.RoundToInt(projectileBounce.Stat.Value),
                smartBounce.BoolSwitch.Active,
                Mathf.RoundToInt(projectilePierce.Stat.Value),
                direction.normalized, shootForce, source);
            direction = Quaternion.AngleAxis(angleBetweenProjectiles.Stat.Value, Vector3.up) * direction;
        }
    }
}
