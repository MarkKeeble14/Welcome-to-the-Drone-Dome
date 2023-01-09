using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper
{
    public static Transform GetClosestTransformToTransform(Transform transform, Collider[] cols)
    {
        if (cols.Length == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                < Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }

    public static Transform GetFurthestTransformToTransform(Transform transform, Collider[] cols)
    {
        if (cols.Length == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                > Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }

    public static Transform GetClosestTransformToTransform(Transform transform, List<Collider> cols)
    {
        if (cols.Count == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                < Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }

    public static Transform GetFurthestTransformToTransform(Transform transform, List<Collider> cols)
    {
        if (cols.Count == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                > Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }
}