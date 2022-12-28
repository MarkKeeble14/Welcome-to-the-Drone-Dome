using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Type Gun", menuName = "Guns/Bullet Type Gun", order = 1)]
public class BulletTypeGun : Gun
{
    [Header("BulletType")]
    [SerializeField] protected BulletTypeProjectile projectilePrefab;
    [SerializeField] protected StatModifier projectilesPerShot;
    [SerializeField] private StatModifier angleBetweenProjectiles;
    [SerializeField] protected StatModifier projectileDamage;
    [SerializeField] protected float shootForce;

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt)
    {
        // If shootAt is null, it means that whatever we were planning on shooting is no longer with us
        // Return 0 as the gun cd, so system can re-target and re-fire
        if (shootAt == null) return 0;

        if (projectilesPerShot.Value == 1)
        {
            ShootOne(projectileOrigin, shootAt);
        }
        else if (projectilesPerShot.Value % 2 == 0)
        {
            ShootEven(projectileOrigin, shootAt);
        }
        else if (projectilesPerShot.Value % 2 == 1)
        {
            ShootOdd(projectileOrigin, shootAt);
        }

        return base.Shoot(projectileOrigin, shootAt);
    }

    private void ShootOne(Vector3 projectileOrigin, Transform shootAt)
    {
        BulletTypeProjectile currentProjectile = Instantiate(projectilePrefab, projectileOrigin, Quaternion.identity);
        Vector3 direction = shootAt.position - projectileOrigin;
        currentProjectile.Set(projectileDamage.Value, direction.normalized, shootForce);
    }

    private void ShootEven(Vector3 projectileOrigin, Transform shootAt)
    {
        Vector3 direction = shootAt.position - projectileOrigin;
        direction = Quaternion.AngleAxis(((projectilesPerShot.Value / 2) - .5f) * -angleBetweenProjectiles.Value, Vector3.up) * direction;

        for (int i = 0; i < projectilesPerShot.Value; i++)
        {
            BulletTypeProjectile currentProjectile = Instantiate(projectilePrefab, projectileOrigin, Quaternion.identity);

            currentProjectile.Set(projectileDamage.Value, direction.normalized, shootForce);
            direction = Quaternion.AngleAxis(angleBetweenProjectiles.Value, Vector3.up) * direction;
        }
    }

    private void ShootOdd(Vector3 projectileOrigin, Transform shootAt)
    {
        Vector3 direction = shootAt.position - projectileOrigin;
        direction = Quaternion.AngleAxis(Mathf.Floor(projectilesPerShot.Value / 2) * -angleBetweenProjectiles.Value, Vector3.up) * direction;

        for (int i = 0; i < projectilesPerShot.Value; i++)
        {
            BulletTypeProjectile currentProjectile = Instantiate(projectilePrefab, projectileOrigin, Quaternion.identity);

            currentProjectile.Set(projectileDamage.Value, direction.normalized, shootForce);
            direction = Quaternion.AngleAxis(angleBetweenProjectiles.Value, Vector3.up) * direction;
        }
    }
}
