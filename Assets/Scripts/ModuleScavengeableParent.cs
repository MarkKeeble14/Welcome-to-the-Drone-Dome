using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleScavengeableParent : MonoBehaviour
{
    [SerializeField] private ModuleScavengeable moduleScavengeable;

    public void SetFromOptions(List<ModuleType> possibleModules)
    {
        moduleScavengeable.SetFromOptions(possibleModules);
    }
}
