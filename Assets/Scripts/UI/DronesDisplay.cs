using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronesDisplay : MonoBehaviour
{
    [SerializeField] private DroneDisplayUnit UIPrefab;

    private Dictionary<DroneController, DroneDisplayUnit> uiDictionary = new Dictionary<DroneController, DroneDisplayUnit>();

    private DroneDisplayUnit currentlySelectedUnit;

    public void AddDisplay(DroneController droneController, PlayerDroneOrbitController playerDroneOrbitController)
    {
        DroneDisplayUnit spawned = Instantiate(UIPrefab, transform);
        spawned.Set(droneController, playerDroneOrbitController);
        uiDictionary.Add(droneController, spawned);
    }

    public void RemoveDisplay(DroneController droneController)
    {
        Destroy(uiDictionary[droneController].gameObject);
        uiDictionary.Remove(droneController);
    }

    public void ShowSelectedDrone(DroneController selectedDrone)
    {
        // Deselect old unit
        if (currentlySelectedUnit != null) currentlySelectedUnit.Selected = false;
        // Select new Unit
        currentlySelectedUnit = uiDictionary[selectedDrone];
        currentlySelectedUnit.Selected = true;
    }

    public void DeselectSelectedDrone()
    {
        currentlySelectedUnit.Selected = false;
        currentlySelectedUnit = null;
    }
}
