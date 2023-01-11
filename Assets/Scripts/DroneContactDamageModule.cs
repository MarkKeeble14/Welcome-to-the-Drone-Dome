using System.Collections;
using System.Linq;
using UnityEngine;

public class DroneContactDamageModule : DronePassiveModule
{
    [SerializeField] private LoadStatModifierInfo damage;
    [SerializeField] private LoadStatModifierInfo sameTargetCD;
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
        hb.Damage(damage.Stat.Value, Type);
        sameTargetCDDictionary.Add(other.gameObject, sameTargetCD.Stat.Value);
    }

    private void Update()
    {
        sameTargetCDDictionary.Update();
    }

    protected override void LoadModuleData()
    {
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
        sameTargetCD.SetStat(UpgradeNode.GetStatModifierUpgradeNode(sameTargetCD, allModuleUpgradeNodes));
    }
}
