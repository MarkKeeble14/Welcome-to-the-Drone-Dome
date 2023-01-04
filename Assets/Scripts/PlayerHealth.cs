using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBehaviour
{
    [Header("Player Health Behaviour")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] private Color hasIFrameColor;
    private Color defaultColor;
    [SerializeField] private LayerMask enemyLayer;
    private bool hasIFrames;

    [Header("Knockback")]
    [SerializeField] private bool knockbackInRadiusWhenHit;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackRadius;
    [SerializeField] private LayerMask knockbackCanHit;

    [Header("References")]
    [SerializeField] private Material material;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private Collider col;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Bar healthBar;


    private new void Start()
    {
        base.Start();

        // Set Variables
        material = new Material(material);
        renderer.material = material;
        defaultColor = material.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, enemyLayer)) return;

        // Get Enemy Component and deal contact damage
        Enemy enemy = null;
        if ((enemy = collision.gameObject.GetComponent<Enemy>()) != null)
        {
            // Apply Knockback
            Vector3 knockbackVector = transform.position - collision.transform.position;
            rb.AddForce(knockbackVector * enemy.KnockbackStrength, ForceMode.Impulse);

            // Apply Damage
            Damage(enemy.ContactDamage, true);
        }
    }

    public override void Damage(float damage, bool spawnText)
    {
        // Technically unneccessary since we're disabling the players collider while they have active I-Frames
        if (hasIFrames) return;

        base.Damage(damage, spawnText);

        if (knockbackInRadiusWhenHit)
            Knockback();

        StartCoroutine(TrackIFrames());
    }

    private void Knockback()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, knockbackRadius, knockbackCanHit);
        foreach (Collider col in hit)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb == null) return;
            rb.AddExplosionForce(knockbackStrength, transform.position, knockbackRadius, 0, ForceMode.Impulse);
        }
    }

    protected override void Die()
    {
        GameManager._Instance.OnPlayerDie();
        base.Die();
    }

    private new void Update()
    {
        base.Update();

        // col.enabled = !hasIFrames;
        // rb.isKinematic = hasIFrames;

        if (hasIFrames)
        {
            material.color = hasIFrameColor;
        }
        else
        {
            material.color = defaultColor;
        }

        healthBar.SetBar(currentHealth / maxHealth);
    }

    private IEnumerator TrackIFrames()
    {
        hasIFrames = true;

        yield return new WaitForSeconds(iFrameDuration);

        hasIFrames = false;
    }
}
