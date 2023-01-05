using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDroneController : MonoBehaviour
{
    [SerializeField] private List<DroneController> trackedDrones = new List<DroneController>();
    public List<DroneController> TrackedDrones { get { return trackedDrones; } }
    [SerializeField] private DroneController selectedDrone;
    public DroneController SelectedDrone => selectedDrone;
    public bool UnderDroneLimit { get { return trackedDrones.Count - 1 < maxDrones; } }

    [Header("Settings")]
    [SerializeField] private int maxDrones = 5;

    private PlayerDroneOrbitController playerDroneOrbitController;
    private Dictionary<DroneController, DroneData> droneDataDict = new Dictionary<DroneController, DroneData>();
    private DronesDisplay DronesDisplay
    {
        get
        {
            return UIManager._Instance.CurrentDroneDisplay;
        }
    }


    public List<DroneModule> AppliedModules
    {
        get
        {
            List<DroneModule> moduleTypes = new List<DroneModule>();
            foreach (DroneController drone in trackedDrones)
            {
                foreach (DroneModule module in drone.AppliedModules)
                {
                    if (!moduleTypes.Contains(module))
                        moduleTypes.Add(module);
                }
            }
            return moduleTypes;
        }
    }

    private void Awake()
    {
        playerDroneOrbitController = GetComponent<PlayerDroneOrbitController>();
    }

    // Adds a drone to the orbiting list and sets anything that needs to be set to do so
    public void AddDroneToOrbit(DroneController drone)
    {
        // Keep track of all drones that have been orbiting
        if (!trackedDrones.Contains(drone))
        {
            trackedDrones.Add(drone);

            // Add new display
            DronesDisplay.AddDisplay(drone, trackedDrones.Count);
        }

        // Add to dictionary
        if (!droneDataDict.ContainsKey(drone))
            droneDataDict.Add(drone, new DroneData(drone, drone.GetComponent<DroneAttackTargeting>()));

        playerDroneOrbitController.AddDroneToOrbit(drone);
    }

    public DroneData ReleaseControlOfSelectedDrone(bool canUseScavengers)
    {
        // If no drone selected, return new DroneData so CursorManager knows there's nothing selected
        if (selectedDrone == null
            || (!canUseScavengers && selectedDrone.CurrentMode == DroneMode.SCAVENGE)
            || !selectedDrone.AvailableForUse)
        {
            return new DroneData();
        }

        // Go through with giving control of the drone over to the CursorManager
        DroneController manipDrone = selectedDrone;
        DroneData toReturn = droneDataDict[manipDrone];

        // Remove that drone from the players orbit
        playerDroneOrbitController.RemoveDroneFromOrbit(manipDrone);

        return toReturn;
    }

    public void TrySelectDrone(DroneController drone)
    {
        // Debug.Log("Trying to Select: " + drone);
        if (!trackedDrones.Contains(drone)) return;
        // Debug.Log("Past Has Check");

        if (selectedDrone == null)
        {
            // Debug.Log("Selected Drone is Null");

            // Select
            selectedDrone = drone;
            DronesDisplay.ShowSelectedDrone();
        }
        else
        {
            // Debug.Log("Selected Drone is not Null");

            if (selectedDrone == drone)
            {
                // Deselect the drone currently selected
                DronesDisplay.DeselectSelectedDrone();
                selectedDrone = null;
            }
            else
            {
                // Deselect the drone currently selected
                DronesDisplay.DeselectSelectedDrone();
                selectedDrone = null;

                // Select
                selectedDrone = drone;
                DronesDisplay.ShowSelectedDrone();
            }
        }
    }

    private void TrySelectDrone(int keyPressed)
    {
        // Check to make sure collection is large enough
        if (trackedDrones.Count < keyPressed) return;

        // Decide what player wants to do
        if (selectedDrone == trackedDrones[keyPressed - 1])
        {
            // Deselect the drone currently selected
            DronesDisplay.DeselectSelectedDrone();
            selectedDrone = null;
        }
        else
        {
            // Select
            selectedDrone = trackedDrones[keyPressed - 1];
            DronesDisplay.ShowSelectedDrone();
        }
    }

    private void TryCycleSelectedDroneMode()
    {
        // Make sure we've actually selected something
        if (selectedDrone == null) return;

        // Cycles the selected drones mode (FOLLOW or SCAVENGE so far)
        selectedDrone.CycleDroneMode();
    }

    private void TryActivateSelectedDroneActive()
    {
        if (selectedDrone == null) return;
        if (!selectedDrone.AvailableForUse) return;
        if (!selectedDrone.CanActivateActives) return;
        selectedDrone.ActivateActives();
    }

    public void ResetDroneActiveCooldowns()
    {
        foreach (DroneController drone in trackedDrones)
        {
            drone.ResetActiveCooldown();
        }
    }

    public void ResetDrones()
    {
        foreach (DroneController drone in trackedDrones)
        {
            drone.ResetState();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add Controls
        InputManager._Controls.Player.CycleDroneMode.started += CycleSelectedDroneModePressed;
        InputManager._Controls.Player.Active.started += ActivateDroneActive;
        InputManager._Controls.Player.TestBinding1.started += Key1Pressed;
        InputManager._Controls.Player.TestBinding2.started += Key2Pressed;
        InputManager._Controls.Player.TestBinding3.started += Key3Pressed;
        InputManager._Controls.Player.TestBinding4.started += Key4Pressed;
        InputManager._Controls.Player.TestBinding5.started += Key5Pressed;
    }

    public void ResetDronePositions()
    {
        foreach (DroneController drone in trackedDrones)
        {
            drone.transform.position = Vector3.zero + Vector3.up * GameManager._Instance.DroneSpawnHeight;
        }
    }

    public void Key1Pressed(InputAction.CallbackContext ctx)
    {
        TrySelectDrone(1);
    }

    public void Key2Pressed(InputAction.CallbackContext ctx)
    {
        TrySelectDrone(2);
    }

    public void Key3Pressed(InputAction.CallbackContext ctx)
    {
        TrySelectDrone(3);
    }

    public void Key4Pressed(InputAction.CallbackContext ctx)
    {
        TrySelectDrone(4);
    }


    public void Key5Pressed(InputAction.CallbackContext ctx)
    {
        TrySelectDrone(5);
    }

    public void CycleSelectedDroneModePressed(InputAction.CallbackContext ctx)
    {
        TryCycleSelectedDroneMode();
    }

    public void ActivateDroneActive(InputAction.CallbackContext ctx)
    {
        TryActivateSelectedDroneActive();
    }
}
