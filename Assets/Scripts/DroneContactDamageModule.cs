using System.Collections;
using System.Linq;
using UnityEngine;

public class DroneContactDamageModule : DroneModule
{
    [SerializeField] private float damage = .25f;
    [SerializeField] private float sameTargetCD = 0.05f;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();
    private string[] canHitLayerStrings = new string[] { "Enemy" };
    private LayerMask canHitLayers;

    private void Start()
    {
        // Get LayerMask
        canHitLayers = LayerMask.GetMask(canHitLayerStrings);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, canHitLayers)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        hb.Damage(damage);
        sameTargetCDDictionary.Add(other.gameObject, sameTargetCD);
    }

    private void Update()
    {
        sameTargetCDDictionary.Update();
    }
}
