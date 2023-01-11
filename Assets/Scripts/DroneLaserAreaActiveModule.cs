using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DroneLaserAreaActiveModule : DroneActiveModule
{
    [SerializeField] private LoadStatModifierInfo range;
    [SerializeField] private LoadStatModifierInfo damage;
    private LayerMask enemyLayer;

    public override ModuleType Type => ModuleType.LASER_ACTIVE;

    private new void Start()
    {
        base.Start();

        // Get enemy layermask
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public override void Effect()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range.Stat.Value, enemyLayer);
        foreach (Collider col in inRange)
        {
            // Get health component
            HealthBehaviour hb = col.GetComponent<HealthBehaviour>();

            // Check to make sure enemy has health
            if (hb == null) continue;

            // Create Laser
            LineBetween spawned = ObjectPooler.laserBeamPool.Get();
            spawned.Set(transform.position, col.transform.position, () => ObjectPooler.laserBeamPool.Release(spawned));

            // Do Damage
            hb.Damage(damage.Stat.Value, Type);
        }
    }

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        range.SetStat(UpgradeNode.GetStatModifierUpgradeNode(range, allModuleUpgradeNodes));
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
    }
}
