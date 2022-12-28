using System;
using UnityEngine;

public class TransformHelper
{
    public static Transform GetClosestTransformToTransform(Transform transform, Collider[] cols)
    {
        if (cols.Length == 0) return null;
        if (cols.Length == 1) return cols[0].transform;

        Transform closestTransform = cols[0].transform;
        for (int i = 1; i < cols.Length; i++)
        {
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                < Vector3.Distance(transform.position, closestTransform.position))
            {
                closestTransform = cols[i].transform;
            }
        }
        return closestTransform;
    }

    public static Transform GetFurthestTransformToTransform(Transform transform, Collider[] cols)
    {
        if (cols.Length == 0) return null;
        if (cols.Length == 1) return cols[0].transform;

        Transform closestTransform = cols[0].transform;
        for (int i = 1; i < cols.Length; i++)
        {
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                > Vector3.Distance(transform.position, closestTransform.position))
            {
                closestTransform = cols[i].transform;
            }
        }
        return closestTransform;
    }
}