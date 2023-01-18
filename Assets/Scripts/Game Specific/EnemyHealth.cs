using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : HealthBehaviour
{
    [Header("Enemy Health Behaviour")]
    [SerializeField] private DropMap dropMap;

    [SerializeField] private float flashDuration = 0.025f;
    [SerializeField] private Color flashColor;
    [SerializeField] private Color defaultColor;

    [Header("References")]
    [SerializeField] private Material material;
    [SerializeField] private new Renderer renderer;
    private Material defaultMaterial;

    private new void Start()
    {
        maxHealth *= GameManager._Instance.EnemyStatMap.MaxHealthMod.Value;

        base.Start();

        // Trying to deal with pesky bug
        defaultMaterial = renderer.material;
        OnDie += () => renderer.material = defaultMaterial;

        // Set Variables
        material = new Material(material);
        material.color = defaultColor;
        renderer.material = material;
    }

    public override void Damage(float damage, bool spawnText)
    {
        base.Damage(damage, spawnText);

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
        dropMap.DropModules(transform.position);
        dropMap.DropUpgradeNodes(transform.position);
        dropMap.DropHearts(transform.position);
        dropMap.DropResources(transform.position);

        base.Die();
    }
}
