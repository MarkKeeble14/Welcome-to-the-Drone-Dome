using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DronesDisplay : MonoBehaviour
{
    [SerializeField] private DroneDisplayUnit UIPrefab;

    private Dictionary<DroneController, DroneDisplayUnit> uiDictionary = new Dictionary<DroneController, DroneDisplayUnit>();

    private DroneDisplayUnit currentlySelectedUnit;

    [Header("References")]
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private ShowSelectedDronesModulesDisplay showSelectedDronesModules;

    public void Set()
    {
        Clear();
        List<DroneController> cachedTrackedDrones = playerDroneController.TrackedDrones;
        for (int i = 0; i < cachedTrackedDrones.Count; i++)
        {
            AddDisplay(cachedTrackedDrones[i], i + 1);
        }
        ShowSelectedDrone();
    }

    private void Clear()
    {
        while (uiDictionary.Count > 0)
        {
            RemoveDisplay(uiDictionary.Keys.ElementAt(0));
        }
    }

    public void AddDisplay(DroneController droneController, int index)
    {
        DroneDisplayUnit spawned = Instantiate(UIPrefab, transform);
        spawned.Set(droneController, playerDroneController, index);
        uiDictionary.Add(droneController, spawned);
    }

    public void RemoveDisplay(DroneController droneController)
    {
        Destroy(uiDictionary[droneController].gameObject);
        uiDictionary.Remove(droneController);
    }

    public void ShowSelectedDrone()
    {
        DroneController selectedDrone = playerDroneController.SelectedDrone;
        if (selectedDrone == null) return;
        // Deselect old unit
        if (currentlySelectedUnit != null) currentlySelectedUnit.Selected = false;
        // Select new Unit
        currentlySelectedUnit = uiDictionary[selectedDrone];
        currentlySelectedUnit.Selected = true;

        // For Shop?
        showSelectedDronesModules.gameObject.SetActive(true);
        showSelectedDronesModules.Set(selectedDrone.AppliedModules);
    }

    public void DeselectSelectedDrone()
    {
        showSelectedDronesModules.gameObject.SetActive(false);

        currentlySelectedUnit.Selected = false;
        currentlySelectedUnit = null;
    }
}
