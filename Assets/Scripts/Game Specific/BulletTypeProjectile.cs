using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTypeProjectile : Projectile
{
    [Header("Projectile Properties")]
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private LayerMask ignoreCollisionWithLayers;

    [Header("Contact Damage")]
    private float contactDamage;

    [Header("Bouncing")]
    [SerializeField] private float smartBouncingDetectionRange = 3f;
    private bool setSmartBounce;
    private int setNumBounces;

    [Header("Piercing")]
    private int setNumCanPierceThrough;

    [Header("Projectile Code Related")]
    [SerializeField] private Rigidbody rb;
    private float speed;
    private Vector3 direction;
    private ModuleType source;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip contactClip;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if object that was collided with belongs to a layer that we have set to ignore
        if (LayerMaskHelper.IsInLayerMask(other.gameObject, damageLayer))
        {
            // Debug.Log("Collision with: " + other.gameObject + " (DamageLayer)");
            // Do damage if hitting something that can take damage
            HealthBehaviour hitHP = null;
            if ((hitHP = other.gameObject.GetComponent<HealthBehaviour>()) != null)
            {
                hitHP.Damage(contactDamage, source);
            }
            HandleContact();
        }
        else if (!LayerMaskHelper.IsInLayerMask(other.gameObject, ignoreCollisionWithLayers))
        {
            // Debug.Log("Collision with: " + other.gameObject + " (Not in IgnoreCollisionsWithLayer)");
            ReleaseAction?.Invoke();
            return;
        }
    }

    private void FixedUpdate()
    {
        // Move in set direction
        if (direction != Vector3.zero)
        {
            rb.velocity = (speed * Time.fixedDeltaTime) * direction;
        }
    }

    public void Set(float damage, int numBounce, bool smartBounce, int numPierce, Vector3 direction, float speed, ModuleType source)
    {
        // Set stats
        contactDamage = damage;
        setNumBounces = numBounce;
        setNumCanPierceThrough = numPierce;
        setSmartBounce = smartBounce;

        // Set movement
        this.direction = direction;
        this.speed = speed;

        // Set source
        this.source = source;

        // Rotate to face direction
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void HandleContact()
    {
        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(contactClip);

        // If we can pierce, do that first
        // If we cannot pierce, but can bounce, do that
        // If we cannot do either, release back to pool
        if (setNumCanPierceThrough > 0)
        {
            setNumCanPierceThrough--;

            // Remove y component of direction
            Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
            direction = newDirection;
        }
        else if (setNumBounces > 0)
        {
            if (setSmartBounce)
            {
                BounceToNearbyEnemy();
            }
            else
            {
                RandomBounce();
            }
            setNumBounces--;
        }
        else
        {
            ReleaseAction?.Invoke();
        }

        // Rotate to face direction
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void RandomBounce()
    {
        Vector3 newDirection = UnityEngine.Random.insideUnitSphere.normalized;
        newDirection = new Vector3(newDirection.x, 0, newDirection.z);
        direction = newDirection;
    }

    private void BounceToNearbyEnemy()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, smartBouncingDetectionRange, damageLayer);

        if (inRange.Length > 0)
        {
            Vector3 newDirection = transform.position - inRange[0].transform.position;
            direction = newDirection;
        }
        else
        {
            RandomBounce();
        }
    }
}
