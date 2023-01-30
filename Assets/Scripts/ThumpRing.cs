using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumpRing : MonoBehaviour
{
    [SerializeField] private Material thumpMaterial;
    [SerializeField] private int pointsCount = 50;
    [SerializeField] private float startWidth = .5f;
    [SerializeField] private LineRenderer lineRenderer;
    private LayerMask enemyLayer;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitClip;

    private void Awake()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public IEnumerator ExecuteThump(Action onEnd, float maxRadius, float expansionSpeed, float damage, float knockback)
    {
        float currentRadius = 0f;

        // Add a Line Renderer for this thump to use so we can have multiple thumps running at the same time if needed
        lineRenderer.material = thumpMaterial;
        lineRenderer.positionCount = pointsCount + 1;

        List<Collider> affectedColliders = new List<Collider>();

        // Expand
        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * expansionSpeed;
            Expand(currentRadius, lineRenderer, maxRadius);
            affectedColliders = Effect(currentRadius, affectedColliders, knockback, damage);
            yield return null;
        }

        onEnd();
    }

    private List<Collider> Effect(float currentRadius, List<Collider> affectedColliders, float knockbackForce, float damage)
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, currentRadius, enemyLayer);

        foreach (Collider col in hit)
        {
            if (affectedColliders.Contains(col)) continue;

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
            sfxSource.PlayOneShot(hitClip);

            affectedColliders.Add(col);

            col.GetComponent<HealthBehaviour>().Damage(damage, ModuleType.SHOCKWAVE_MORTAR);

            if (knockbackForce != 0)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (!rb) continue;
                Vector3 direction = (col.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
            }
        }

        return affectedColliders;
    }

    private void Expand(float currentRadius, LineRenderer lineRenderer, float maxRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for (int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            Vector3 position = direction * currentRadius;

            lineRenderer.SetPosition(i, transform.position + position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }
}
