using System.Collections;
using System.Linq;
using UnityEngine;

public class DroneContactDamageModule : DronePassiveModule
{
    [SerializeField] private LoadStatModifierInfo damage;
    [SerializeField] private LoadStatModifierInfo sameTargetCD;
    [SerializeField] private DroneContactDamageHitbox hitbox;
    public override ModuleType Type => ModuleType.DRONE_CONTACT_DAMAGE;

    private void Start()
    {
        hitbox.Set(damage.Stat.Value, sameTargetCD.Stat.Value);
    }

    protected override void LoadModuleData()
    {
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
        sameTargetCD.SetStat(UpgradeNode.GetStatModifierUpgradeNode(sameTargetCD, allModuleUpgradeNodes));
    }
}
