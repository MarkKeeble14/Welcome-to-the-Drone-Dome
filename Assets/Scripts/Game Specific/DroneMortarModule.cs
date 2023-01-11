using UnityEngine;

public abstract class DroneMortarModule : DroneGunModule
{
    [Header("Mortar")]
    [SerializeField] protected LoadStatModifierInfo projectileSpeed;
    [SerializeField] private float arcAngle = 60f;
    [SerializeField] private float torquePower;

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        projectileSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(projectileSpeed, allModuleUpgradeNodes));
    }

    protected abstract void SetWeaponSpecificProjectileInfo(Projectile proj);

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        MortarProjectile currentProjectile = (MortarProjectile)ObjectPooler._Instance.GetProjectile(source);
        SetWeaponSpecificProjectileInfo(currentProjectile);
        currentProjectile.transform.position = projectileOrigin;
        currentProjectile.Set(shootAt, projectileSpeed.Stat.Value, arcAngle);

        // TODO: Problem here; Trying to release an object that has already been released to the pool
        currentProjectile.ReleaseAction = () => ObjectPooler._Instance.ReleaseProjectile(source, currentProjectile);

        if (torquePower > 0)
        {
            Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
            rb.AddTorque(Random.insideUnitSphere.normalized * torquePower);
        }

        return base.Shoot(projectileOrigin, shootAt, source);
    }
}