using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneZapperModule : DroneWeaponModule
{
    [Header("Zapper Module")]
    [SerializeField] protected StatModifier range;
    [SerializeField] protected StatModifier delay;
    [SerializeField] protected StatModifier damage;
    [SerializeField] protected StatModifier chainAmount;
    [SerializeField] private LineRenderer chainLightning;
    private LineRenderer spawned;
    private float forceDestroyTimer;
    private float gracePeriodModifier = 2f;
    public override ModuleType Type => ModuleType.ZAPPER;

    private void Update()
    {
        if (forceDestroyTimer <= 0)
        {
            DestroySpawned();
        }
        else
        {
            forceDestroyTimer -= Time.deltaTime;
        }
    }

    public override IEnumerator Attack()
    {
        DestroySpawned();
        DoDamage();
        yield return new WaitForSeconds(delay.Value);
        StartCoroutine(Attack());
    }

    private void DestroySpawned()
    {
        if (spawned == null) return;

        Destroy(spawned.gameObject);
    }

    protected void DoDamage()
    {
        forceDestroyTimer = delay.Value * gracePeriodModifier;

        Vector3 checkOrigin = transform.position;

        List<Collider> hasHit = new List<Collider>();
        spawned = Instantiate(chainLightning, transform);

        // For the number of chaining available
        for (int i = 0; i <= chainAmount.Value; i++)
        {
            // Check for something in range
            Collider[] inRange = Physics.OverlapSphere(checkOrigin, range.Value, enemyLayer);
            // If nothing, exit
            if (inRange.Length == 0) return;

            Collider target = null;
            for (int k = 0; k < inRange.Length; k++)
            {
                if (!hasHit.Contains(inRange[k]))
                {
                    target = inRange[k];
                    break;
                }
            }

            // Nothing to hit
            if (target == null) return;

            // Can hit the currently selected target
            hasHit.Add(target);

            // If something, set checkOrigin to new target (moving origin), and set the line rendererer accordingly
            if (i == 0)
            {
                spawned.positionCount++;
                spawned.SetPosition(0, transform.position);
            }
            spawned.positionCount++;
            checkOrigin = target.transform.position;
            spawned.SetPosition(i + 1, checkOrigin);

            // If has health, deal damage
            HealthBehaviour hb = null;
            if ((hb = inRange[0].GetComponent<HealthBehaviour>()) != null)
            {
                hb.Damage(damage.Value, ModuleType.ZAPPER);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range.Value);
    }
}
