using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTypeProjectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    [SerializeField] private LayerMask collideWithLayers;

    [Header("Contact Damage")]
    [SerializeField] private bool hasContactDamage;
    private float contactDamage;

    [Header("Bouncing")]
    [SerializeField] private bool smartBouncingEnabled;
    [SerializeField] private float smartBouncingDetectionRange;
    [SerializeField] private int numBounces;

    [Header("Piercing")]
    [SerializeField] private int numCanPierceThrough = 0;

    [Header("Lifetime")]
    [SerializeField] private bool shouldDestroyAfterTime;
    [SerializeField] private float lifetime = 5;

    private float speed;
    private Vector2 direction;

    [Header("Projectile Code Related")]
    [SerializeField] private Rigidbody rb;
    private LayerMask enemyLayer;

    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        if (shouldDestroyAfterTime)
            StartCoroutine(DestroyAfterTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log(collision.gameObject.name);
        // Checks if object that was collided with belongs to a layer that we have set to ignore
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, collideWithLayers)) return;

        // Do damage if hitting something that can take damage
        HealthBehaviour hitHP = null;
        if ((hitHP = collision.gameObject.GetComponent<HealthBehaviour>()) != null)
        {
            hitHP.Damage(contactDamage);
        }
        Contact();
    }

    public void Set(float damage, Vector3 direction, float speed)
    {
        if (hasContactDamage)
        {
            contactDamage = damage;
        }
        StartCoroutine(Travel(speed, direction));
    }

    private void Contact()
    {
        // Bounces are consumed first
        if (numBounces > 0)
        {
            if (smartBouncingEnabled)
                BounceToNearbyEnemy();
            else
                RandomBounce();
            numBounces--;
        }
        else if (numCanPierceThrough <= 0)
        {
            Destroy(gameObject);
        }
        else if (numCanPierceThrough > 0)
        {
            numCanPierceThrough--;
        }
        else if (numBounces <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void RandomBounce()
    {
        StopCoroutine(Travel(speed, direction));
        Vector2 newDirection = Random.insideUnitCircle.normalized;
        StartCoroutine(Travel(speed, new Vector3(newDirection.x, 0, newDirection.y)));
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
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
