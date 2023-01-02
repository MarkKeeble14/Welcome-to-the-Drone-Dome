using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UseDroneManager : MonoBehaviour
{
    public static UseDroneManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [SerializeField] private LayerMask targetable;
    [SerializeField] private LayerMask shoveable;
    [SerializeField] private LayerMask ground;

    [SerializeField] private float droneShoveDelay = .5f;
    [SerializeField] private float droneShoveMaxRadiusMod = 2f;
    Vector2 mousePos;

    [SerializeField] private PlayerDroneController playerDroneController;

    private void LeftMousePressed(InputAction.CallbackContext obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager._Controls.Player.MousePosition.ReadValue<Vector2>());
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, shoveable);

        if (hit.collider != null)
        {
            StartCoroutine(StartDrag(hit.collider.transform));
        }
        else
        {
            TryClick();
        }
    }

    private IEnumerator StartDrag(Transform targetedObject)
    {
        // Lot's of checking if can do action

        // Debug.Log("Targeted: " + targetedObject);

        // Start the Shoving Logic
        // Stops the player grid from controlling the drone
        DroneData droneData = playerDroneController.ReleaseControlOfSelectedDrone(false);

        // If droneData struct is empty, cancel sequence; 
        // Similarily if player has tried to command a scavenging drone to attack
        if (droneData.DroneController == null
            || droneData.DroneController.CurrentMode == DroneMode.SCAVENGE)
        {
            yield break;
        };

        DroneController controllingDrone = droneData.DroneController;

        // Check to make sure selected drone is strong enough to shove
        Shoveable targetedShoveable = null;
        if ((targetedShoveable = targetedObject.GetComponent<Shoveable>()) != null)
        {
            if (targetedShoveable.ShoveRequirement > controllingDrone.ShoveLimit)
                yield break;
        }

        // Confirmed can go through with the action, begin the actual logic for the shove
        controllingDrone.AvailableForUse = false;
        controllingDrone.DisableAttackModules();

        // Disable Collider
        controllingDrone.Col.enabled = false;

        // Dragging around
        while (InputManager._Controls.Player.LeftMouseClick.IsPressed())
        {
            // Cancelling drag; reset
            if (targetedObject == null || controllingDrone.CurrentMode == DroneMode.SCAVENGE)
            {
                // End and Reset Settings

                // Add the drone back to the player's orbit
                playerDroneController.AddDroneToOrbit(droneData.DroneController);

                // Allow ambient attacking
                controllingDrone.DisableAttackModules();

                // Re-enable Collider
                controllingDrone.Col.enabled = true;

                yield break;
            }

            // Debug.Log("Left Mouse Held");
            Ray ray = Camera.main.ScreenPointToRay(InputManager._Controls.Player.MousePosition.ReadValue<Vector2>());
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
            if (hit.collider == null)
            {
                Debug.Log("Missed ground; can't aim");
                yield break;
            }
            Vector3 mouseReleasePos = hit.point;
            mouseReleasePos.y = targetedObject.position.y;

            // Set drones position
            controllingDrone.transform.position
                = GetDroneCirclingPosition(targetedObject, mouseReleasePos, controllingDrone);

            yield return null;
        }

        targetedShoveable.Primed = true;

        // Add Force
        Rigidbody rb = targetedObject.GetComponent<Rigidbody>();
        rb.AddForce(
            Vector3.Distance(targetedObject.position, controllingDrone.transform.position)
                * controllingDrone.ShoveStrength
                * (targetedObject.position - controllingDrone.transform.position).normalized,
            ForceMode.Impulse);

        // Play animation
        // ?

        // Wait a moment
        yield return new WaitForSeconds(droneShoveDelay);

        // Add the drone back to the player's orbit
        playerDroneController.AddDroneToOrbit(droneData.DroneController);

        // Allow ambient attacking
        controllingDrone.EnableAttackModules();
        controllingDrone.AvailableForUse = true;

        // Re-enable Collider
        controllingDrone.Col.enabled = true;
    }

    // Clamps the drone's position to be on a circles edge around the given anchor, with some min and max radius
    private Vector3 GetDroneCirclingPosition(Transform anchor, Vector3 mousePos, DroneController drone)
    {
        float minRadius = 1; // Inner radius
        float maxRadius = 1 * droneShoveMaxRadiusMod; // Outer radius
        Vector3 centerPosition = anchor.position; // Center position
        float distance = Vector3.Distance(mousePos, centerPosition); // Distance from mouse to object
        Vector3 position = mousePos; // Default position to mousePos; if nothing needs to change about it, it's within
        // the bounds already

        // If the distance is too small (mouse is too close to object)
        if (distance < minRadius)
        {
            Vector3 fromOriginToObject = mousePos - centerPosition; // Find vector between objects
            fromOriginToObject *= minRadius / distance; //Multiply by radius, then Divide by distance
            position = centerPosition + fromOriginToObject; // Add new vector to anchor position
        }
        else if (distance > maxRadius) // If the distance is too large (mouse is too far from object)
        {
            Vector3 fromOriginToObject = mousePos - centerPosition; // Math is the same as above
            fromOriginToObject *= maxRadius / distance;
            position = centerPosition + fromOriginToObject;
        }
        return position;
    }

    private void TryClick()
    {
        mousePos = InputManager._Controls.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, targetable);
        if (hit.collider == null) return;
        switch (hit.collider.gameObject.layer)
        {
            case 7:
                StartCoroutine(ClickEnemySequence(hit.collider.gameObject));
                break;
            default:
                break;
        }
    }

    private IEnumerator ClickEnemySequence(GameObject enemy)
    {
        // Stops the player grid from controlling the drone
        DroneData droneData = playerDroneController.ReleaseControlOfSelectedDrone(false);

        // If droneData struct is empty, cancel sequence; 
        // Similarily if player has tried to command a scavenging drone to attack
        if (droneData.DroneController == null
            || droneData.DroneController.CurrentMode == DroneMode.SCAVENGE)
        {
            yield break;
        };

        DroneController controllingDrone = droneData.DroneController;

        // Get's the enemy orbit grid component
        DroneAttackingEnemyOrbitController dAtkECont = enemy.GetComponent<DroneAttackingEnemyOrbitController>();

        // Disable Collider
        controllingDrone.Col.enabled = false;

        // Add the drone to the enemies orbit grid
        dAtkECont.AddDroneToOrbit(controllingDrone);

        // Sets the drone to focus one target
        droneData.DroneTargeting.OverridingTarget = dAtkECont.transform;

        // Wait until enemy has died
        yield return new WaitUntil(() => dAtkECont == null
        || controllingDrone.CurrentMode == DroneMode.SCAVENGE);

        // Add the drone back to the player's orbit
        playerDroneController.AddDroneToOrbit(droneData.DroneController);

        // Make the drone no longer focus the dead target
        droneData.DroneTargeting.OverridingTarget = null;

        // Re-enable Collider
        controllingDrone.Col.enabled = true;
    }
    private void Start()
    {
        InputManager._Controls.Player.LeftMouseClick.started += LeftMousePressed;
    }
}
