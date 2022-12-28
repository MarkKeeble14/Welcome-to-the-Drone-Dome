using System.Collections.Generic;
using UnityEngine;

public partial class EntityDroneOrbitController : MonoBehaviour
{
    [Header("Entity Orbit Controller")]
    [SerializeField] protected List<DroneController> orbitingDrones = new List<DroneController>();
    protected Dictionary<DroneController, DroneData> droneDataDict = new Dictionary<DroneController, DroneData>();

    [SerializeField] protected OrbitPlacementGrid orbitGrid;

    // Adds a drone to the list and sets anything that needs to be set to do so
    public virtual void AddDroneToOrbit(DroneController drone)
    {
        // Create new node and tell drone to follow it
        Transform node = orbitGrid.TryGetUnfollowedNode(orbitingDrones);
        if (node == null)
            node = orbitGrid.AddNode();

        drone.Follow = node;

        orbitingDrones.Add(drone);

        // Add to dictionary
        if (!droneDataDict.ContainsKey(drone))
            droneDataDict.Add(drone, new DroneData(drone, drone.GetComponent<DroneAttackTargeting>()));
    }

    // Removes a drone from the list and sets anything that needs to be set to do so
    public virtual void RemoveDroneFromOrbit(DroneController drone)
    {
        if (orbitingDrones.Count <= 0) return;

        // Remove node and tell drone to no longer follow one
        orbitGrid.RemoveNode(drone.Follow);
        drone.Follow = null;
        orbitingDrones.Remove(drone);
    }
}
