using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DroneLaserAreaActiveModule : DroneActiveModule
{
    [SerializeField] private StatModifier range;
    [SerializeField] private StatModifier damage;
    [SerializeField] private LineBetween laser;
    private LayerMask enemyLayer;

    public override ModuleType Type => ModuleType.LASER_ACTIVE;

    private void Start()
    {
        // Get enemy layermask
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public override void Effect()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range.Value, enemyLayer);
        foreach (Collider col in inRange)
        {
            // Get health component
            HealthBehaviour hb = col.GetComponent<HealthBehaviour>();
            // Check to make sure enemy has health
            if (hb == null) continue;
            // Do Damage
            hb.Damage(damage.Value, Type);
            // Create Laser
            Instantiate(laser, transform.position, Quaternion.identity).Set(transform.position, col.transform.position);
        }
    }
}
