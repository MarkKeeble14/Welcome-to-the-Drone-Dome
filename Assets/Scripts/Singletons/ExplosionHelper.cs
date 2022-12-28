using UnityEngine;

public class ExplosionHelper : MonoBehaviour
{
    public static ExplosionHelper _Instance;

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }

        // Set instance
        _Instance = this;
    }

    public static void ExplodeAt(ExplosionData explosionData, Vector3 position)
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
                    healthBehaviour.Damage(explosionData.Damage);
                }
            }

            // Explode other object
            Explodable explodable;
            if ((explodable = hit.GetComponent<Explodable>()) && explodable.AllowChainExplosion)
            {
                explodable.Explode();
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
}
