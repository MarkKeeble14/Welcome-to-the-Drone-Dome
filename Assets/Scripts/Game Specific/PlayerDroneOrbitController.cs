using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerDroneOrbitController : EntityDroneOrbitController
{
    // Adds a drone to the orbiting list and sets anything that needs to be set to do so
    public override void AddDroneToOrbit(DroneController drone)
    {
        base.AddDroneToOrbit(drone);
    }

    // Removes a drone from the orbiting list and sets anything that needs to be set to do so
    public override void RemoveDroneFromOrbit(DroneController drone)
    {
        base.RemoveDroneFromOrbit(drone);
    }
}
