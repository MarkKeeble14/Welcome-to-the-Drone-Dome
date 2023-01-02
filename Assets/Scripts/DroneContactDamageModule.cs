using System.Collections;
using System.Linq;
using UnityEngine;

public class DroneContactDamageModule : DronePassiveModule
{
    [SerializeField] private float damage = .25f;
    [SerializeField] private float sameTargetCD = 0.05f;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();
    private LayerMask enemyLayer;

    public override ModuleType Type => ModuleType.DRONE_CONTACT_DAMAGE;

    private void Start()
    {
        // Get LayerMask
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, enemyLayer)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        hb.Damage(damage, Type);
        sameTargetCDDictionary.Add(other.gameObject, sameTargetCD);
    }

    private void Update()
    {
        sameTargetCDDictionary.Update();
    }
}
