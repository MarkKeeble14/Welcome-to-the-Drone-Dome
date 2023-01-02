using UnityEngine;

[CreateAssetMenu(fileName = "Burst Fire Gun", menuName = "Guns/Burst Fire Gun", order = 1)]
public class BurstFireGun : BulletTypeGun
{
    [Header("Burst Fire")]
    [SerializeField] private StatModifier projectilesInBurst;
    [SerializeField] private float timeBetweenBullets;

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        BurstFireGunHelper._Instance.CallBurstFire(
            () => base.Shoot(projectileOrigin, shootAt, source),
            timeBetweenBullets,
            projectilesInBurst.Value);
        return (1 / shotsPerSecond.Value);
    }
}