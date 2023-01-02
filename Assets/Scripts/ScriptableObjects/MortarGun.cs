using UnityEngine;

[CreateAssetMenu(fileName = "Mortar Gun", menuName = "Guns/Mortar Gun", order = 1)]
public class MortarGun : Gun
{
    [Header("Burst Fire")]
    [SerializeField] private MortarProjectile mortarProjectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float arcAngle;
    [SerializeField] private float torquePower;

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        MortarProjectile currentProjectile = Instantiate(mortarProjectilePrefab, projectileOrigin, Quaternion.identity);
        currentProjectile.Set(shootAt, projectileSpeed, arcAngle);

        if (torquePower > 0)
        {
            Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
            rb.AddTorque(Random.insideUnitSphere.normalized * torquePower);
        }

        return base.Shoot(projectileOrigin, shootAt, source);
    }
}