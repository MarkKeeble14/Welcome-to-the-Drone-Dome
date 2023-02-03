using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicFieldTriggerField : ModuleDamageTriggerField
{
    [Header("Poison Field")]
    [SerializeField] private bool expand;
    [SerializeField] private float expandSpeed;

    public override ModuleType Source => ModuleType.TOXIC_SHELL_MORTAR;

    public void Set(bool expand, float expandSpeed)
    {
        this.expand = expand;
        this.expandSpeed = expandSpeed;
    }

    private void OnEnable()
    {
        StartCoroutine(Expand());
    }

    private new void Start()
    {
        base.Start();
        StartCoroutine(Expand());
    }

    private IEnumerator Expand()
    {
        // Debug.Log("Start Expanse");
        while (expand)
        {
            if (!reachedMaxRadius) yield return null;

            // Debug.Log("Expanding");
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, transform.localScale + Vector3.one, expandSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
