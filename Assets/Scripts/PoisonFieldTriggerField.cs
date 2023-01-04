using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFieldTriggerField : ModuleDamageTriggerField
{
    [Header("Poison Field")]
    [SerializeField] private BoolSwitchUpgradeNode expand;
    [SerializeField] private StatModifier expandSpeed;

    public override ModuleType Source => ModuleType.TOXIC_SHELL_MORTAR;

    private new void Start()
    {
        StartCoroutine(Expand());

        base.Start();
    }

    private IEnumerator Expand()
    {
        // Debug.Log("Start Expanse");
        while (expand.Active)
        {
            if (!reachedMaxRadius) yield return null;

            // Debug.Log("Expanding");
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, transform.localScale + Vector3.one, expandSpeed.Value * Time.deltaTime);

            yield return null;
        }
    }
}
