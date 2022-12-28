using System.Collections;
using UnityEngine;

public class EnemyHealth : HealthBehaviour
{
    [Header("Enemy Health Behaviour")]
    [SerializeField] private Vector2 minMaxCanDrop;
    [SerializeField] private GameObject dropOnDeath;

    [SerializeField] private float flashDuration = 0.025f;
    [SerializeField] private Color flashColor;
    private Color defaultColor;

    [Header("References")]
    [SerializeField] private Material material;
    [SerializeField] private new Renderer renderer;

    private new void Start()
    {
        base.Start();

        // Set Variables
        material = new Material(material);
        renderer.material = material;
        defaultColor = material.color;
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        // If enemy is already flashing, stop existing coroutine so as not to interrupt the one we're about to start
        if (material.color != defaultColor)
            StopAllCoroutines();
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        material.color = defaultColor;
    }

    protected override void Die()
    {
        // Drop XP
        int numToDrop = RandomHelper.RandomIntExclusive(minMaxCanDrop);
        for (int i = 0; i < numToDrop; i++)
        {
            Instantiate(dropOnDeath, transform.position, Quaternion.identity);
        }
        base.Die();
    }
}
