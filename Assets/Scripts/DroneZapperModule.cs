using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneZapperModule : DroneWeaponModule
{
    [Header("Zapper Module")]
    [SerializeField] protected StatModifier fireRange;
    [SerializeField] protected StatModifier chainRange;
    [SerializeField] protected StatModifier delay;
    [SerializeField] protected StatModifier damage;
    [SerializeField] protected StatModifier chainAmount;

    public override ModuleType Type => ModuleType.ZAPPER;

    public override IEnumerator Attack()
    {
        yield return new WaitForSeconds(delay.Value);
        DoDamage();
        StartCoroutine(Attack());
    }

    protected void DoDamage()
    {
        List<Transform> hasTouched = new List<Transform>();
        hasTouched.Add(transform);
        LineBetween spawned;

        // For the number of chaining available
        for (int i = 0; i < chainAmount.Value; i++)
        {
            target = targeting.GetTarget((i == 0 ? fireRange.Value : chainRange.Value), hasTouched[i], TargetBy, hasTouched);
            // Nothing to hit
            if (target == null)
                break;

            // Can hit the currently selected target
            hasTouched.Add(target);

            // If has health, deal damage
            HealthBehaviour hb = null;
            if ((hb = target.GetComponent<HealthBehaviour>()) != null)
            {
                hb.Damage(damage.Value, ModuleType.ZAPPER);
            }
        }

        spawned = ObjectPooler.chainLightningPool.Get();
        spawned.Set(hasTouched, () => ObjectPooler.chainLightningPool.Release(spawned));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fireRange.Value);
    }
}
