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
    [SerializeField] private Transform cursorHovering;
    [SerializeField] private Vector3 cursorWorldPos;

    [Header("Audio")]
    [SerializeField] private AudioClip droneShoveClip;
    [SerializeField] private AudioClip droneStartDragClip;
    [SerializeField] private AudioClip droneTargetClip;

    private bool CanStartShove(DroneController drone)
    {
        return (drone.CurrentMode == DroneMode.ATTACK || drone.CurrentMode == DroneMode.SCAVENGE)
            && CursorOverShoveable() && drone.AvailableForUse;
    }

    private bool CursorOverShoveable()
    {
        if (cursorHovering == null) return false;
        return LayerMaskHelper.IsInLayerMask(cursorHovering.gameObject, shoveable);
    }

    private bool CanClickEnemy(DroneController drone)
    {
        return drone.CurrentMode == DroneMode.ATTACK
            && CursorOverEnemy() && drone.AvailableForUse;
    }

    private bool CursorOverEnemy()
    {
        if (cursorHovering == null) return false;
        return LayerMaskHelper.IsInLayerMask(cursorHovering.gameObject, targetable);
    }

    private bool CanStation(DroneController drone)
    {
        return playerDroneController.SelectedDrone.CurrentMode == DroneMode.STATION && drone.AvailableForUse;
    }

    private void LeftMousePressed(InputAction.CallbackContext obj)
    {
        if (cursorHovering == null) return;

        // Stuff for Attack mode
        if (playerDroneController.SelectedDrone != null)
        {
            if (CanStartShove(playerDroneController.SelectedDrone))
            {
                StartCoroutine(StartDrag(cursorHovering));
            }
            else if (CanClickEnemy(playerDroneController.SelectedDrone))
            {
                StartCoroutine(ClickEnemySequence(cursorHovering.gameObject));
            }
            else if (CanStation(playerDroneController.SelectedDrone))
            {
                playerDroneController.SelectedDrone.SetStation(cursorWorldPos);
            }
        }
    }

    private IEnumerator StartDrag(Transform targetedObject)
    {
        // Lot's of checking if can do action

        // Debug.Log("Targeted: " + targetedObject);

        // Start the Shoving Logic
        // Stops the player grid from controlling the drone
        DroneData droneData = playerDroneController.ReleaseControlOfSelectedDrone(true);
        DroneController usingDrone = droneData.DroneController;

        // If droneData struct is empty, cancel sequence; 
        if (usingDrone == null)
            yield break;

        // Check to make sure selected drone is strong enough to shove selected object
        Shoveable targetedShoveable = null;
        if ((targetedShoveable = targetedObject.GetComponent<Shoveable>()) == null)
        {
            yield break;
        }

        // Confirmed can go through with the action, begin the actual logic for the shove
        MouseCursorManager._Instance.SetCursor(CursorType.HAND, true);

        usingDrone.AvailableForUse = false;

        // Audio
        AudioManager._Instance.PlayClip(droneStartDragClip, RandomHelper.RandomFloat(.8f, 1.2f), usingDrone.transform.position);

        Transform lockedTarget = targetedObject;

        // Dragging around
        while (InputManager._Controls.Player.LeftMouseClick.IsPressed())
        {
            // Cancelling drag; reset
            if (lockedTarget == null)
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

        targetedShoveable.SetPrimed(true);

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

            // Audio
            AudioManager._Instance.PlayClip(droneShoveClip, RandomHelper.RandomFloat(.8f, 1.2f), rb.transform.position, .75f);
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

        // Audio
        AudioManager._Instance.PlayClip(droneTargetClip, RandomHelper.RandomFloat(.8f, 1.2f), controllingDrone.transform.position, .9f);

        // Add the drone to the enemies orbit grid
        controllingDrone.SetRotateAround(enemy.transform);

        // Sets the drone to focus one target
        droneData.DroneTargeting.OverridingTarget = enemy.transform;

        // Wait until enemy has died
        yield return new WaitUntil(() => enemy == null
        || controllingDrone.CurrentMode == DroneMode.SCAVENGE); ;

        // Add the drone back to the player's orbit
        playerDroneController.AddDroneToOrbit(droneData.DroneController);

        // Make the drone no longer focus the dead target
        droneData.DroneTargeting.OverridingTarget = null;
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

        if (playerDroneController.SelectedDrone != null)
        {
            if (CanClickEnemy(playerDroneController.SelectedDrone))
            {
                MouseCursorManager._Instance.SetCursor(CursorType.TARGET, false);
            }
            else if (CanStartShove(playerDroneController.SelectedDrone))
            {
                MouseCursorManager._Instance.SetCursor(CursorType.HAND, false);
            }
            else
            {
                MouseCursorManager._Instance.SetCursor(CursorType.DEFAULT, false);
            }
        }
        else
        {
            MouseCursorManager._Instance.SetCursor(CursorType.DEFAULT, false);
        }
    }
}
