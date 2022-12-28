using UnityEngine;

public class DroneBlockBulletsModule : DroneModule
{
    private float blockingRadius;
    private LayerMask bulletLayer;
    private SphereCollider sphereCollider;

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
