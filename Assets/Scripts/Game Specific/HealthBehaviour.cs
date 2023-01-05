﻿using System;
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

    [SerializeField] protected PopupText popupText;

    public Action OnDie { get; set; }

    protected void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damage, ModuleType source)
    {
        Instantiate(popupText, transform.position, Quaternion.identity)
            .Set(damage, GameManager._Instance.GetModuleColor(source),
            (transform.position.y + transform.localScale.y / 2));
        Damage(damage, false);
    }

    public virtual void Damage(float damage, bool spawnText, Color popUpTextColor)
    {
        Instantiate(popupText, transform.position, Quaternion.identity)
            .Set(damage, popUpTextColor,
            (transform.position.y + transform.localScale.y / 2));
        Damage(damage, false);
    }

    public virtual void Damage(float damage, bool spawnText)
    {
        currentHealth -= damage;
        Instantiate(takeDamageParticleEffect, transform.position, Quaternion.identity);
        if (!spawnText) return;
        Instantiate(popupText, transform.position, Quaternion.identity)
            .Set(damage, Color.white, (transform.position.y + transform.localScale.y / 2));
    }

    public virtual void Heal(float healAmount, bool spawnText)
    {
        if (currentHealth >= maxHealth) return;
        currentHealth += healAmount;
        Instantiate(healParticleEffect, transform.position, Quaternion.identity);
        if (!spawnText) return;
        Instantiate(popupText, transform.position, Quaternion.identity)
            .Set("+", healAmount, Color.green, (transform.position.y + transform.localScale.y / 2));
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
        OnDie?.Invoke();
        Instantiate(dieParticleEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}