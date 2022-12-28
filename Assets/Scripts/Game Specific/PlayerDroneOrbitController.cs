using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerDroneOrbitController : EntityDroneOrbitController
{
    [Header("Player Orbit Controller")]
    [SerializeField] private List<DroneController> trackedDrones = new List<DroneController>();
    public List<DroneController> TrackedDrones { get { return trackedDrones; } }
    [SerializeField] private DronesDisplay dronesDisplay;
    private DroneController selectedDrone;

    public bool UnderDroneLimit { get { return trackedDrones.Count - 1 < maxDrones; } }

    [Header("Settings")]
    [SerializeField] private int maxDrones = 5;
    [SerializeField] private int numDronesToStart = 1;

    // Adds a drone to the orbiting list and sets anything that needs to be set to do so
    public override void AddDroneToOrbit(DroneController drone)
    {
        // Keep track of all drones that have been orbiting
        if (!trackedDrones.Contains(drone))
        {
            trackedDrones.Add(drone);

            // Add new display
            dronesDisplay.AddDisplay(drone, this);
        }

        base.AddDroneToOrbit(drone);
    }

    // Removes a drone from the orbiting list and sets anything that needs to be set to do so
    public override void RemoveDroneFromOrbit(DroneController drone)
    {
        base.RemoveDroneFromOrbit(drone);

        // Remove display
        // dronesDisplay.RemoveDisplay(drone);
    }

    public DroneData ReleaseControlOfSelectedDrone(bool canUseScavengers)
    {
        // If no drone selected, return new DroneData so CursorManager knows there's nothing selected
        if (selectedDrone == null
            || (!canUseScavengers && selectedDrone.CurrentMode == DroneMode.SCAVENGE)
            || !selectedDrone.AvailableForUse)
            return new DroneData();

        // Go through with giving control of the drone over to the CursorManager
        DroneController manipDrone = selectedDrone;
        DroneData toReturn = droneDataDict[manipDrone];

        // Remove that drone from the players orbit
        RemoveDroneFromOrbit(manipDrone);

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
            dronesDisplay.ShowSelectedDrone(selectedDrone);
        }
        else
        {
            // Debug.Log("Selected Drone is not Null");

            if (selectedDrone == drone)
            {
                // Deselect the drone currently selected
                dronesDisplay.DeselectSelectedDrone();
                selectedDrone = null;
            }
            else
            {
                // Deselect the drone currently selected
                dronesDisplay.DeselectSelectedDrone();
                selectedDrone = null;

                // Select
                selectedDrone = drone;
                dronesDisplay.ShowSelectedDrone(selectedDrone);
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
            dronesDisplay.DeselectSelectedDrone();
            selectedDrone = null;
        }
        else
        {
            // Select
            selectedDrone = trackedDrones[keyPressed - 1];
            dronesDisplay.ShowSelectedDrone(selectedDrone);
        }
    }

    private void TryCycleSelectedDroneMode()
    {
        // Make sure we've actually selected something
        if (selectedDrone == null) return;

        // Cycles the selected drones mode (FOLLOW or SCAVENGE so far)
        selectedDrone.CycleDroneMode();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add Controls
        InputManager._Controls.Player.CycleDroneMode.started += CycleSelectedDroneModePressed;
        InputManager._Controls.Player.TestBinding1.started += Key1Pressed;
        InputManager._Controls.Player.TestBinding2.started += Key2Pressed;
        InputManager._Controls.Player.TestBinding3.started += Key3Pressed;
        InputManager._Controls.Player.TestBinding4.started += Key4Pressed;
        InputManager._Controls.Player.TestBinding5.started += Key5Pressed;

        // Give player starting drones
        for (int i = 0; i < numDronesToStart; i++)
        {
            GameManager._Instance.SpawnAndAddDroneToOrbit();
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
}
