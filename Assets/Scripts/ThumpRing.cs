using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumpRing : MonoBehaviour
{
    [SerializeField] private Material thumpMaterial;
    [SerializeField] private int pointsCount = 50;
    [SerializeField] private StatModifier maxRadius;
    [SerializeField] private StatModifier speed;
    [SerializeField] private float startWidth = .5f;
    [SerializeField] private StatModifier thumpDamage;
    [SerializeField] private StatModifier knockbackForce;
    [SerializeField] private LineRenderer lineRenderer;
    private LayerMask enemyLayer;

    private void Awake()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public IEnumerator ExecuteThump(bool destroyOnComplete)
    {
        float currentRadius = 0f;

        // Add a Line Renderer for this thump to use so we can have multiple thumps running at the same time if needed
        lineRenderer.material = thumpMaterial;
        lineRenderer.positionCount = pointsCount + 1;

        List<Collider> affectedColliders = new List<Collider>();

        // Expand
        while (currentRadius < maxRadius.Value)
        {
            currentRadius += Time.deltaTime * speed.Value;
            Expand(currentRadius, lineRenderer);
            affectedColliders = Effect(currentRadius, affectedColliders);
            yield return null;
        }

        Destroy(gameObject);
        if (destroyOnComplete)
            Destroy(transform.parent.gameObject);
    }

    private List<Collider> Effect(float currentRadius, List<Collider> affectedColliders)
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, currentRadius, enemyLayer);

        foreach (Collider col in hit)
        {
            if (affectedColliders.Contains(col)) continue;

            affectedColliders.Add(col);

            col.GetComponent<HealthBehaviour>().Damage(thumpDamage.Value, ModuleType.THUMPER_MORTAR);

            if (knockbackForce.Value != 0)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (!rb) continue;
                Vector3 direction = (col.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce.Value, ForceMode.Impulse);
            }
        }

        return affectedColliders;
    }

    private void Expand(float currentRadius, LineRenderer lineRenderer)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for (int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            Vector3 position = direction * currentRadius;

            lineRenderer.SetPosition(i, transform.position + position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius.Value);
    }
}
