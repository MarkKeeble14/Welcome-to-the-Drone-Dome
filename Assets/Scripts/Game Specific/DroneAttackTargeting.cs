using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneAttackTargeting : MonoBehaviour
{
    public Transform OverridingTarget;

    [SerializeField] private LayerMask enemyLayer;

    public Transform GetTarget(float range, Transform t, WeaponTargetingType targetBy)
    {
        if (OverridingTarget != null)
            return OverridingTarget;
        Collider[] inRange = Physics.OverlapSphere(t.position, range, enemyLayer);
        if (inRange.Length == 0) return null;
        switch (targetBy)
        {
            case WeaponTargetingType.ANY:
                return inRange[0].transform;
            case WeaponTargetingType.CLOSEST:
                return TransformHelper.GetClosestTransformToTransform(t, inRange);
            case WeaponTargetingType.FURTHEST:
                return TransformHelper.GetFurthestTransformToTransform(t, inRange);
            default:
                return null;
        }
    }

    public Transform GetTarget(float range, Transform t, WeaponTargetingType targetBy, List<Transform> exclude)
    {
        if (OverridingTarget != null && !exclude.Contains(OverridingTarget))
            return OverridingTarget;
        List<Collider> inRange = Physics.OverlapSphere(t.position, range, enemyLayer).ToList();
        List<Collider> candidateTargets = new List<Collider>();

        if (inRange.Count == 0) return null;

        foreach (Collider col in inRange)
        {
            if (!exclude.Contains(col.transform))
                candidateTargets.Add(col);
        }

        switch (targetBy)
        {
            case WeaponTargetingType.ANY:
                return candidateTargets[0].transform;
            case WeaponTargetingType.CLOSEST:
                return TransformHelper.GetClosestTransformToTransform(t, candidateTargets);
            case WeaponTargetingType.FURTHEST:
                return TransformHelper.GetFurthestTransformToTransform(t, candidateTargets);
            default:
                return null;
        }
    }
}
