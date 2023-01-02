using System.Collections;
using UnityEngine;

public abstract class MortarProjectile : MonoBehaviour
{
    private LayerMask explodeOnCollideWith;

    private void Awake()
    {
        explodeOnCollideWith = LayerMask.GetMask("Enemy", "Ground");
    }

    public void Set(Transform shootAt, float speed, float arcAngle)
    {
        ShootAt(shootAt, arcAngle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnCollideWith)) return;
        ArrivedAtPosition();
    }

    public abstract void ArrivedAtPosition();

    private void ShootAt(Transform shootAt, float arcAngle)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 p = shootAt.position;

        float gravity = Physics.gravity.magnitude;

        // Selected angle in radians
        float angle = arcAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        // rb.velocity = finalVelocity;

        // Alternative way:
        rb.AddForce(finalVelocity * rb.mass, ForceMode.Impulse);
    }
}
