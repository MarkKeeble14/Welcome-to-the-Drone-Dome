﻿using System;
using UnityEngine;

public class ExplosionHelper : MonoBehaviour
{
    public static ExplosionHelper _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        // Set instance
        _Instance = this;
    }

    public static void ExplodeAt(ExplosionData explosionData, Vector3 position, Action<HealthBehaviour> onHitHealthBehaviour)
    {
        // Play explosion sfx
        if (explosionData.AudioEffect != null)
            AudioSource.PlayClipAtPoint(explosionData.AudioEffect, position);

        // Instantiate particles
        GameObject spawned = Instantiate(explosionData.VisualEffect, position, Quaternion.identity);
        spawned.transform.localScale = Vector3.one * MathHelper.Normalize(explosionData.Radius, 1, 10, .25f, 2f);

        // Get an array of enemies within explosion
        Collider[] colliders = Physics.OverlapSphere(position, explosionData.Radius);

        foreach (Collider hit in colliders)
        {
            // Deal Damage if collider has either an EnemyHealth component or PlayerHealth component
            // Also explode other crates if neccessary
            if (LayerMaskHelper.IsInLayerMask(hit.gameObject, explosionData.DealDamageTo))
            {
                HealthBehaviour healthBehaviour;
                if ((healthBehaviour = hit.GetComponent<HealthBehaviour>()))
                {
                    // Debug.Log(hit);
                    // healthBehaviour.Damage(explosionData.Damage, true);
                    onHitHealthBehaviour(healthBehaviour);
                }
            }

            // Explode other object
            Explodable explodable;
            if ((explodable = hit.GetComponent<Explodable>()) && explodable.AllowChainExplosion)
            {
                explodable.CallExplode();
            }

            Rigidbody rb;
            if ((rb = hit.GetComponent<Rigidbody>()))
            {
                // Apply Force to all colliders hit that have rigidbody
                // Debug.Log(hit + ", " + rb);
                rb.AddExplosionForce(explosionData.Power, position, explosionData.Radius, explosionData.Radius);
            }

        }
    }

    public static void ExplodeEnemiesAt(ExplosionData explosionData, Vector3 position, ModuleType source)
    {
        ExplodeAt(explosionData, position, enemy => enemy.Damage(explosionData.Damage, source));
    }

    public static void ExplodeEnemiesAt(ExplosionData explosionData, Vector3 position)
    {
        ExplodeAt(explosionData, position, enemy => enemy.Damage(explosionData.Damage, true, Color.red));
    }
}
