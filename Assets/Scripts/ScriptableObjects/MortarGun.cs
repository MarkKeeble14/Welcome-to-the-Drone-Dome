using UnityEngine;

[CreateAssetMenu(fileName = "Mortar Gun", menuName = "Guns/Mortar Gun", order = 1)]
public class MortarGun : Gun
{
    [Header("Mortar")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float arcAngle;
    [SerializeField] private float torquePower;

    public override float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        MortarProjectile currentProjectile = (MortarProjectile)ObjectPooler._Instance.GetProjectile(source);
        currentProjectile.transform.position = projectileOrigin;
        currentProjectile.Set(shootAt, projectileSpeed, arcAngle);

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