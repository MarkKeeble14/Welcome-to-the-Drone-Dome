using System.Collections.Generic;
using UnityEngine;

public class DroneAttackTargeting : MonoBehaviour
{
    public Transform OverridingTarget;

    [SerializeField] private LayerMask enemyLayer;

    public Transform GetTarget(float range, WeaponTargetingType targetBy)
    {
        if (OverridingTarget != null)
            return OverridingTarget;
        Collider[] inRange = Physics.OverlapSphere(transform.position, range, enemyLayer);
        if (inRange.Length == 0) return null;
        switch (targetBy)
        {
            case WeaponTargetingType.ANY:
                return inRange[0].transform;
            case WeaponTargetingType.CLOSEST:
                return TransformHelper.GetClosestTransformToTransform(transform, inRange);
            case WeaponTargetingType.FURTHEST:
                return TransformHelper.GetFurthestTransformToTransform(transform, inRange);
            default:
                return inRange[0].transform;
        }
    }
}
