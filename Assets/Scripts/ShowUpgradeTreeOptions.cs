using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowUpgradeTreeOptions : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private Transform droneUpgradesList;
    [SerializeField] private Transform otherUpgradesList;
    [SerializeField] private SelectUpgradeNodeDisplay selectUpgradePrefab;
    private List<SelectUpgradeNodeDisplay> spawned = new List<SelectUpgradeNodeDisplay>();

    public void Set(List<DroneController> drones, List<UpgradeTree> other, Action<DroneController> onPressDrone, Action<UpgradeTree> onPressOther)
    {
        // Remove previously spawned
        foreach (SelectUpgradeNodeDisplay node in spawned)
        {
            Destroy(node.gameObject);
        }
        spawned.Clear();

        // Spawn Drone Nodes
        foreach (DroneController drone in drones)
        {
            SelectUpgradeNodeDisplay node = Instantiate(selectUpgradePrefab, droneUpgradesList);
            node.SetDrone(drone, onPressDrone);
            spawned.Add(node);
        }

        // Spawn Other Nodes
        foreach (UpgradeTree tree in other)
        {
            SelectUpgradeNodeDisplay node = Instantiate(selectUpgradePrefab, otherUpgradesList);
            node.SetOther(tree, onPressOther);
            spawned.Add(node);
        }
    }
}
