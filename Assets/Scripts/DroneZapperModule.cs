using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneZapperModule : DroneWeaponModule
{
    [Header("Zapper Module")]
    [Header("Upgradeables")]
    [SerializeField] private LoadStatModifierInfo damage;
    [SerializeField] private LoadStatModifierInfo delay;
    [SerializeField] private LoadStatModifierInfo fireRange;
    [SerializeField] private LoadStatModifierInfo chainRange;
    [SerializeField] private LoadStatModifierInfo chainAmount;

    public override ModuleType Type => ModuleType.ZAPPER;

    [SerializeField] private Transform origin;

    [Header("Audio")]
    [SerializeField] private AudioClip zapClip;

    protected override void LoadModuleData()
    {
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
        delay.SetStat(UpgradeNode.GetStatModifierUpgradeNode(delay, allModuleUpgradeNodes));
        fireRange.SetStat(UpgradeNode.GetStatModifierUpgradeNode(fireRange, allModuleUpgradeNodes));
        chainRange.SetStat(UpgradeNode.GetStatModifierUpgradeNode(chainRange, allModuleUpgradeNodes));
        chainAmount.SetStat(UpgradeNode.GetStatModifierUpgradeNode(chainAmount, allModuleUpgradeNodes));
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
        List<Transform> hasTouched = new List<Transform>();
        hasTouched.Add(origin);
        LineBetween spawned;

        // For the number of chaining available
        for (int i = 0; i < chainAmount.Stat.Value; i++)
        {
            target = targeting.GetTarget((i == 0 ? fireRange.Stat.Value : chainRange.Stat.Value), hasTouched[i], TargetBy, hasTouched);
            // Nothing to hit
            if (target == null)
                break;

            // Can hit the currently selected target
            hasTouched.Add(target);

            // If has health, deal damage
            HealthBehaviour hb = null;
            if ((hb = target.GetComponent<HealthBehaviour>()) != null)
            {
                hb.Damage(damage.Stat.Value, ModuleType.ZAPPER);
            }
        }

        // Didn't add any enemies to the list
        if (hasTouched[hasTouched.Count - 1] == origin)
        {
            return;
        }

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(zapClip);

        spawned = ObjectPooler.chainLightningPool.Get();
        spawned.Set(hasTouched, () => ObjectPooler.chainLightningPool.Release(spawned));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin.position, fireRange.Stat.Value);
    }
}
