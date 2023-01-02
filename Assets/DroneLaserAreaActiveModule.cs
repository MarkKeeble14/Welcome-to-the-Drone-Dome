using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DroneLaserAreaActiveModule : DroneActiveModule
{
    private StatModifier range;
    private StatModifier damage;
    private LineBetween laser;
    private LayerMask enemyLayer;

    public override ModuleType Type => ModuleType.LASER_ACTIVE;

    private new void Awake()
    {
        LoadResources();

        enemyLayer = LayerMask.GetMask("Enemy");

        base.Awake();
    }

    private new void Start()
    {
        base.Start();
    }

    private void LoadResources()
    {
        laser = Resources.Load<LineBetween>("LaserActive/Laser");
        cooldownStart = Resources.Load<StatModifier>("LaserActive/Stat/LaserCooldown");
        range = Resources.Load<StatModifier>("LaserActive/Stat/LaserRange");
        damage = Resources.Load<StatModifier>("LaserActive/Stat/LaserDamage");
    }

    public override void Effect()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range.Value, enemyLayer);
        foreach (Collider col in inRange)
        {
            // Get health component
            EnemyHealth enemyHealth = col.GetComponent<EnemyHealth>();
            // Check to make sure enemy has health
            if (enemyHealth == null) continue;
            // Do Damage
            enemyHealth.Damage(damage.Value, Type);
            // Create Laser
            Instantiate(laser, transform.position, Quaternion.identity).Set(transform.position, col.transform.position);
        }
    }
}
