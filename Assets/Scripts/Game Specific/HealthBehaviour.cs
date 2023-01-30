using System;
using UnityEngine;

public class HealthBehaviour : MonoBehaviour
{
    [Header("Base Health Behaviour")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    public float MaxHealth => maxHealth;
    [SerializeField] private GameObject takeDamageParticleEffect;
    [SerializeField] private GameObject healParticleEffect;
    [SerializeField] private GameObject dieParticleEffect;

    public Action OnDie { get; set; }

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip damagedClip;
    [SerializeField] private AudioClip healedClip;
    [SerializeField] private AudioClip dieClip;

    protected void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damage, ModuleType source)
    {
        ObjectPooler.popupTextPool.Get()
            .Set(damage, GameManager._Instance.GetModuleColor(source),
            transform.position + Vector3.up * (transform.position.y + transform.localScale.y / 2));
        Damage(damage, false);
    }

    public virtual void Damage(float damage, bool spawnText, Color popUpTextColor)
    {
        ObjectPooler.popupTextPool.Get()
            .Set(damage, popUpTextColor, transform.position + Vector3.up * (transform.position.y + transform.localScale.y / 2));
        Damage(damage, false);
    }

    // Main Damage Function
    public virtual void Damage(float damage, bool spawnText)
    {
        currentHealth -= damage;
        Instantiate(takeDamageParticleEffect, transform.position, Quaternion.identity);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(damagedClip);

        // Popup Text
        if (!spawnText) return;
        ObjectPooler.popupTextPool.Get()
            .Set(damage, Color.white, transform.position + Vector3.up * (transform.position.y + transform.localScale.y / 2));
    }

    public virtual void Heal(float healAmount, bool spawnText)
    {
        if (currentHealth >= maxHealth) return;
        currentHealth += healAmount;
        Instantiate(healParticleEffect, transform.position, Quaternion.identity);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(healedClip);

        if (!spawnText) return;
        ObjectPooler.popupTextPool.Get()
           .Set("+", healAmount, Color.green, transform.position + Vector3.up * (transform.position.y + transform.localScale.y / 2));
    }

    protected void Update()
    {
        if (currentHealth <= 0)
        {
            // Audio
            AudioManager._Instance.PlayClip(dieClip, true, transform.position);

            Die();
        }
    }

    protected virtual void Die()
    {
        OnDie?.Invoke();
        Instantiate(dieParticleEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}