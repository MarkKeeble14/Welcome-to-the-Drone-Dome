using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneModuleScavengeableParent : MonoBehaviour
{
    [SerializeField] private DroneModuleScavengeable moduleScavengeable;
    [SerializeField] private Vector3 defaultScale;

    public void SetFromOptions(List<ModuleType> possibleModules, Vector3 position)
    {
        // Set type
        moduleScavengeable.Set(possibleModules);

        // Set position of this and children
        transform.position = position;
        foreach (Transform child in transform)
        {
            child.localPosition = Vector3.zero;
        }

        // Reset scale of pickup
        moduleScavengeable.transform.localScale = defaultScale;
    }
}
