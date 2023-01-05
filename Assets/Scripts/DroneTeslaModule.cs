using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTeslaModule : DroneWeaponModule
{
    [Header("Tesla Module")]
    [SerializeField] protected StatModifier range;
    [SerializeField] protected StatModifier delay;
    [SerializeField] protected StatModifier damage;
    // References
    [SerializeField] private LineBetween teslaShock;

    public override ModuleType Type => ModuleType.TESLA_COIL;


    public override IEnumerator Attack()
    {
        DoDamage();
        StartCoroutine(ForceDestroySpawned());
        yield return new WaitForSeconds(delay.Value);
        StartCoroutine(Attack());
    }

    private IEnumerator ForceDestroySpawned()
    {
        yield return new WaitForSeconds(delay.Value);
        DestroySpawned();
    }

    private void DestroySpawned()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    protected void DoDamage()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range.Value, enemyLayer);

        if (inRange.Length == 0) return;

        foreach (Collider c in inRange)
        {
            HealthBehaviour hb = null;
            if ((hb = c.GetComponent<HealthBehaviour>()) != null)
            {
                LineBetween spawned = Instantiate(teslaShock, transform);
                spawned.Set(transform.position, c.transform.position);
                hb.Damage(damage.Value, ModuleType.TESLA_COIL);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range.Value);
    }
}
