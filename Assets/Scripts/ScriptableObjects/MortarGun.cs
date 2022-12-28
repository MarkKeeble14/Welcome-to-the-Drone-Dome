using UnityEngine;

[CreateAssetMenu(fileName = "Mortar Gun", menuName = "Guns/Mortar Gun", order = 1)]
public class MortarGun : Gun
{
    [Header("Burst Fire")]
    [SerializeField] private MortarProjectile mortarProjectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float arcHeight;

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt)
    {
        MortarProjectile currentProjectile = Instantiate(mortarProjectilePrefab, projectileOrigin, Quaternion.identity);
        currentProjectile.Set(shootAt, projectileSpeed, arcHeight);

        return base.Shoot(projectileOrigin, shootAt);
    }
}