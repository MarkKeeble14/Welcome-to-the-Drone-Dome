using UnityEngine;

public class DroneBlockBulletsModule : DronePassiveModule
{
    private float blockingRadius;
    private LayerMask bulletLayer;
    private SphereCollider sphereCollider;

    public override ModuleType Type => ModuleType.DRONE_BLOCK_BULLETS;

    private void Start()
    {
        // Get LayerMask
        bulletLayer = LayerMask.GetMask("EnemyBullet");

        // Get and Set Sphere Collider
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = blockingRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskHelper.IsInLayerMask(other.gameObject, bulletLayer))
        {
            Destroy(other.gameObject);
        }
    }
}
