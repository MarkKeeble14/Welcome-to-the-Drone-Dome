using System.Collections.Generic;
using UnityEngine;

public class DroneModuleUpgradeScavengeableParent : MonoBehaviour
{
    [SerializeField] private DroneModuleUpgradeScavengeable moduleScavengeable;
    [SerializeField] private Vector3 defaultScale;

    public void SetFromOptions(List<UpgradeTree> trees, Vector3 position)
    {
        // Set type
        moduleScavengeable.Set(trees);

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
