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

    [SerializeField] private PlayerDroneController playerDroneController;
    private Transform cursorHovering;
    [SerializeField] private Vector3 cursorWorldPos;

    private void LeftMousePressed(InputAction.CallbackContext obj)
    {
        // Stuff for Station Mode
        if (playerDroneController.SelectedDrone != null && playerDroneController.SelectedDrone.CurrentMode == DroneMode.STATION)
        {
            playerDroneController.SelectedDrone.SetStation(cursorWorldPos);
        }

        if (cursorHovering == null) return;

        // Stuff for Attack mode
        if (playerDroneController.SelectedDrone != null && playerDroneController.SelectedDrone.CurrentMode == DroneMode.ATTACK)
        {
            if (LayerMaskHelper.IsInLayerMask(cursorHovering.gameObject, shoveable))
            {
                StartCoroutine(StartDrag(cursorHovering));
            }
            else if (LayerMaskHelper.IsInLayerMask(cursorHovering.gameObject, targetable))
            {
                StartCoroutine(ClickEnemySequence(cursorHovering.gameObject));
            }
        }
    }

    private IEnumerator StartDrag(Transform targetedObject)
    {
        // Lot's of checking if can do action

        // Debug.Log("Targeted: " + targetedObject);

        // Start the Shoving Logic
        // Stops the player grid from controlling the drone
        DroneData droneData = playerDroneController.ReleaseControlOfSelectedDrone(false);
        DroneController usingDrone = droneData.DroneController;

        // If droneData struct is empty, cancel sequence; 
        if (usingDrone == null)
            yield break;

        // Check to make sure selected drone is strong enough to shove selected object
        Shoveable targetedShoveable = null;
        if ((targetedShoveable = targetedObject.GetComponent<Shoveable>()) != null)
        {
            if (targetedShoveable.ShoveRequirement > usingDrone.ShoveLimit)
            {
                yield break;
            }
        }

        // Confirmed can go through with the action, begin the actual logic for the shove
        MouseCursorManager._Instance.SetCursor(CursorType.HAND, true);

        usingDrone.AvailableForUse = false;
        usingDrone.OnEnterScavengeMode();
        // Disable Collider
        usingDrone.Col.enabled = false;

        // Dragging around
        while (InputManager._Controls.Player.LeftMouseClick.IsPressed())
        {
            // Cancelling drag; reset
            if (targetedObject == null || usingDrone.CurrentMode == DroneMode.SCAVENGE)
            {
                // End and Reset Settings
                ResetDrone(usingDrone);

                yield break;
            }

            // Debug.Log("Left Mouse Held");
            Ray ray = Camera.main.ScreenPointToRay(InputManager._Controls.Player.MousePosition.ReadValue<Vector2>());
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
            if (hit.collider == null)
            {
                ResetDrone(usingDrone);
                yield break;
            }
            Vector3 mouseReleasePos = hit.point;
            mouseReleasePos.y = targetedObject.position.y;

            // Set drones position
            usingDrone.transform.position
                = Vector3.MoveTowards(usingDrone.transform.position,
                GetDroneCirclingPosition(targetedObject, mouseReleasePos, usingDrone), usingDrone.MoveSpeed * 3 * Time.deltaTime);

            yield return null;
        }

        targetedShoveable.Primed = true;

        // The +1 is a little gray range outside of the radius as extra leeway for the player
        if (Vector3.Distance(usingDrone.transform.position, targetedShoveable.transform.position) <= droneShoveMaxRadiusMod + 1)
        {
            // Add Force
            Rigidbody rb = targetedObject.GetComponent<Rigidbody>();
            rb.AddForce(
                Vector3.Distance(targetedObject.position, usingDrone.transform.position)
                    * usingDrone.ShoveStrength
                                * (targetedObject.position - usingDrone.transform.position).normalized,
                            ForceMode.Impulse);
        }
        else
        {
            ResetDrone(usingDrone);
            yield break;
        }

        // Play animation
        // ?

        MouseCursorManager._Instance.SetCursor(CursorType.DEFAULT, false);

        // Wait a moment
        yield return new WaitForSeconds(droneShoveDelay);

        ResetDrone(usingDrone);
    }

    private void ResetDrone(DroneController drone)
    {
        // Allow ambient attacking
        drone.OnEnterAttackMode();
        // Re-enable Collider
        drone.Col.enabled = true;
        drone.AvailableForUse = true;
        // Add the drone back to the player's orbit
        playerDroneController.AddDroneToOrbit(drone);
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

    private void Update()
    {
        Vector2 cursorScreenPos = InputManager._Controls.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(cursorScreenPos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, targetable | shoveable | ground);
        cursorHovering = hit.transform;
        cursorWorldPos = hit.point;

        if (MouseCursorManager._Instance.Locked) return;
        if (playerDroneController.SelectedDrone == null || !playerDroneController.SelectedDrone.AvailableForUse)
        {
            MouseCursorManager._Instance.SetCursor(CursorType.DEFAULT, false);
            return;
        }

        if (cursorHovering == null)
        {
            MouseCursorManager._Instance.SetCursor(CursorType.DEFAULT, false);
            return;
        }

        if (LayerMaskHelper.IsInLayerMask(hit.transform.gameObject, targetable))
        {
            MouseCursorManager._Instance.SetCursor(CursorType.TARGET, false);
        }
        else if (LayerMaskHelper.IsInLayerMask(hit.transform.gameObject, shoveable))
        {
            MouseCursorManager._Instance.SetCursor(CursorType.HAND, false);
        }
    }
}
