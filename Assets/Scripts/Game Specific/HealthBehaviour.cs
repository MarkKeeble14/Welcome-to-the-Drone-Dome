using System;
using UnityEngine;

public class HealthBehaviour : MonoBehaviour
{
    [Header("Base Health Behaviour")]
    [SerializeField] protected float startHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] private GameObject takeDamageParticleEffect;
    [SerializeField] private GameObject dieParticleEffect;

    public Action OnDie { get; set; }

    protected void Start()
    {
        currentHealth = startHealth;
    }

    public virtual void Damage(float damage)
    {
        currentHealth -= damage;
        Instantiate(takeDamageParticleEffect, transform.position, Quaternion.identity);
    }

    protected void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnDie();
        Instantiate(dieParticleEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}