using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DroneTeslaModule : DroneWeaponModule
{
    [Header("Tesla Module")]
    [SerializeField] protected StatModifier range;
    [SerializeField] protected StatModifier delay;
    [SerializeField] protected StatModifier damage;

    [SerializeField] private BoolSwitchUpgradeNode setActiveWhenScavenging;
    public override ModuleType Type => ModuleType.TESLA_COIL;

    private void Update()
    {
        // Allow to still be used in scavenge mode if upgrade is active
        activeWhenScavenging = setActiveWhenScavenging.Active;
    }

    public override IEnumerator Attack()
    {
        yield return new WaitForSeconds(delay.Value);
        DoDamage();
        StartCoroutine(Attack());
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
                LineBetween spawned = ObjectPooler.teslaArcPool.Get();
                spawned.Set(transform.position, c.transform.position, () => ObjectPooler.teslaArcPool.Release(spawned));
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
