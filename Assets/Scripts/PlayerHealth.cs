using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerHealth : HealthBehaviour
{
    [Header("Player Health Behaviour")]
    [SerializeField] private float iFrameDuration;
    private float iFrameDurationTimer;
    [SerializeField] private Color hasIFrameColor;
    private Color defaultColor;
    [SerializeField] private float defaultEmission;
    [SerializeField] private float hasIFrameEmission;
    [SerializeField] private LayerMask enemyLayer;
    public bool HasIFrames => iFrameDurationTimer > 0;

    [Header("Knockback")]
    [SerializeField] private bool knockbackInRadiusWhenHit;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackRadius;
    [SerializeField] private LayerMask knockbackCanHit;

    [Header("References")]
    [SerializeField] private Material material;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Bar healthBar;

    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;

    private int playerLayer;
    private int playerIgnoreEnemyLayer;

    private new void Start()
    {
        base.Start();

        playerLayer = LayerMask.NameToLayer("Player");
        playerIgnoreEnemyLayer = LayerMask.NameToLayer("PlayerIgnoreEnemy");

        // Set Variables
        material = new Material(material);
        renderer.material = material;
        defaultColor = material.color;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (HasIFrames) return;
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

            // Call screenshake
            cinemachineImpulseSource.GenerateImpulse();
        }
    }

    public override void Damage(float damage, bool spawnText)
    {
        // Technically unneccessary since we're disabling the players collider while they have active I-Frames
        if (HasIFrames) return;

        base.Damage(damage, spawnText);

        if (knockbackInRadiusWhenHit)
            Knockback();

        iFrameDurationTimer = iFrameDuration;
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

        if (HasIFrames || playerMovement.Dashing)
        {
            gameObject.layer = playerIgnoreEnemyLayer;

            iFrameDurationTimer -= Time.deltaTime;

            material.color = hasIFrameColor;
            material.SetColor("_EmissionColor", hasIFrameColor * hasIFrameEmission);
        }
        else
        {
            gameObject.layer = playerLayer;
            iFrameDurationTimer = 0;
            material.color = defaultColor;
            material.SetColor("_EmissionColor", defaultColor * defaultEmission);
        }

        healthBar.SetBar(currentHealth / maxHealth);
    }
}
