using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DroneTeslaModule : DroneWeaponModule
{
    [Header("Tesla Module")]
    [Header("Upgradeables")]
    [SerializeField] private LoadStatModifierInfo range;
    [SerializeField] private LoadStatModifierInfo delay;
    [SerializeField] private LoadStatModifierInfo damage;
    [SerializeField] private LoadBoolSwitchInfo canBeActiveWhenScavenging;

    public override ModuleType Type => ModuleType.TESLA_COIL;

    [SerializeField] private Transform origin;

    [Header("Audio")]
    [SerializeField] private AudioClip zapClip;

    private void Update()
    {
        // Allow to still be used in scavenge mode if upgrade is active
        activeWhenScavenging = canBeActiveWhenScavenging.BoolSwitch.Active;
    }

    protected override void LoadModuleData()
    {
        range.SetStat(UpgradeNode.GetStatModifierUpgradeNode(range, allModuleUpgradeNodes));
        delay.SetStat(UpgradeNode.GetStatModifierUpgradeNode(delay, allModuleUpgradeNodes));
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
        canBeActiveWhenScavenging.SetBoolSwitch(UpgradeNode.GetBoolSwitchUpgradeNode(canBeActiveWhenScavenging, allModuleUpgradeNodes));
    }

    public override IEnumerator Attack()
    {
        if (paused) yield return new WaitUntil(() => !paused);
        if (!Attached) yield return null;
        yield return new WaitForSeconds(delay.Stat.Value);
        DoDamage();
        StartCoroutine(Attack());
    }

    protected void DoDamage()
    {
        Collider[] inRange = Physics.OverlapSphere(origin.position, range.Stat.Value, enemyLayer);

        if (inRange.Length == 0) return;

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(zapClip);

        foreach (Collider c in inRange)
        {
            HealthBehaviour hb = null;
            if ((hb = c.GetComponent<HealthBehaviour>()) != null)
            {
                LineBetween spawned = ObjectPooler.teslaArcPool.Get();
                spawned.Set(origin.position, c.transform.position, () => ObjectPooler.teslaArcPool.Release(spawned));
                hb.Damage(damage.Stat.Value, ModuleType.TESLA_COIL);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin.position, range.Stat.Value);
    }
}
