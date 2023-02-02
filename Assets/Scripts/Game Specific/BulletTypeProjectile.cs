using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTypeProjectile : Projectile
{
    [Header("Projectile Properties")]
    [SerializeField] private LayerMask collideWithLayers;

    [Header("Contact Damage")]
    private float contactDamage;

    [Header("Bouncing")]
    [SerializeField] private float smartBouncingDetectionRange = 3f;
    private bool setSmartBounce;
    private int setNumBounces;

    [Header("Piercing")]
    private int setNumCanPierceThrough;

    [Header("Max Lifetime")]
    [SerializeField] private bool shouldDestroyAfterTime;
    private float maxLifetime = 20f;

    private float speed;
    private Vector2 direction;

    [Header("Projectile Code Related")]
    [SerializeField] private Rigidbody rb;
    private LayerMask enemyLayer;
    private ModuleType source;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip contactClip;

    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        if (shouldDestroyAfterTime)
            StartCoroutine(DestroyAfterTime());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if object that was collided with belongs to a layer that we have set to ignore
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, collideWithLayers)) return;

        // Do damage if hitting something that can take damage
        HealthBehaviour hitHP = null;
        if ((hitHP = other.gameObject.GetComponent<HealthBehaviour>()) != null)
        {
            hitHP.Damage(contactDamage, source);
        }
        HandleContact();
    }

    public void Set(float damage, int numBounce, bool smartBounce, int numPierce, Vector3 direction, float speed, ModuleType source)
    {
        // Set stats
        contactDamage = damage;
        setNumBounces = numBounce;
        setNumCanPierceThrough = numPierce;
        setSmartBounce = smartBounce;

        // Set source
        this.source = source;

        // Rotate to face direction
        transform.rotation = Quaternion.LookRotation(direction);

        // Move in direction
        StartCoroutine(Travel(speed, direction));
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
    }

    private void RandomBounce()
    {
        StopCoroutine(Travel(speed, direction));
        Vector3 newDirection = UnityEngine.Random.insideUnitSphere.normalized;
        StartCoroutine(Travel(speed, new Vector3(newDirection.x, 0, newDirection.z)));
    }

    private void BounceToNearbyEnemy()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, smartBouncingDetectionRange, enemyLayer);

        StopCoroutine(Travel(speed, direction));
        if (inRange.Length > 0)
        {
            Vector3 newDirection = transform.position - inRange[0].transform.position;
            StartCoroutine(Travel(speed, newDirection));
        }
        else
        {
            RandomBounce();
        }
    }

    private IEnumerator Travel(float speed, Vector3 direction)
    {
        this.direction = direction;
        this.speed = speed;

        while (true)
        {
            rb.velocity = (speed * Time.deltaTime) * direction;

            yield return null;
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(maxLifetime);

        if (gameObject.activeSelf)
            ReleaseAction?.Invoke();
    }
}
