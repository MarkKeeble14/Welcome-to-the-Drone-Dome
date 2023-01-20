using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneAttackTargeting : MonoBehaviour
{
    public Transform OverridingTarget;

    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        if (OverridingTarget)
        {
            transform.LookAt(OverridingTarget);
        }
    }

    public Transform GetTarget(float range, Transform t, WeaponTargetingType targetBy)
    {
        if (OverridingTarget != null)
            return OverridingTarget;
        Collider[] inRange = Physics.OverlapSphere(t.position, range, enemyLayer);
        if (inRange.Length == 0) return null;
        Transform toReturn = null;
        switch (targetBy)
        {
            case WeaponTargetingType.ANY:
                toReturn = inRange[0].transform;
                break;
            case WeaponTargetingType.CLOSEST:
                toReturn = TransformHelper.GetClosestTransformToTransform(t, inRange);
                break;
            case WeaponTargetingType.FURTHEST:
                toReturn = TransformHelper.GetFurthestTransformToTransform(t, inRange);
                break;
            default:
                toReturn = null;
                break;
        }
        transform.LookAt(toReturn);
        return toReturn;
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

        Transform toReturn = null;
        switch (targetBy)
        {
            case WeaponTargetingType.ANY:
                toReturn = candidateTargets[0].transform;
                break;
            case WeaponTargetingType.CLOSEST:
                toReturn = TransformHelper.GetClosestTransformToTransform(t, candidateTargets);
                break;
            case WeaponTargetingType.FURTHEST:
                toReturn = TransformHelper.GetFurthestTransformToTransform(t, candidateTargets);
                break;
            default:
                toReturn = null;
                break;
        }
        transform.LookAt(toReturn);
        return toReturn;
    }
}
