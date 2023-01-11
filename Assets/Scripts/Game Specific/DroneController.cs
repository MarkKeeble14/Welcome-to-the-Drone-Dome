﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Core")]
    public DroneMode CurrentMode;
    public bool AvailableForUse = true;

    public float ShoveStrength => droneBasics.ShoveStrength.Stat.Value;
    public float MoveSpeed => droneBasics.MoveSpeed.Stat.Value;
    public float OrbitDistance => droneBasics.OrbitDistance.Stat.Value;
    public float OrbitSpeed => droneBasics.OrbitSpeed.Stat.Value;
    public float ScavengeableSightRange => droneBasics.ScavengeableSightRange.Stat.Value;
    public float ScavengingGrabRange => droneBasics.ScavengingGrabRange.Stat.Value;
    public float ScavengingSpeedMod => droneBasics.ScavengingSpeedMod.Stat.Value;

    [Header("Orbit")]
    [SerializeField] private Transform rotateAround;
    private float orbitTimer;

    [Header("Scavenging")]
    [SerializeField] private LayerMask scavengeableLayer;
    [SerializeField] private LayerMask scavengeablePriorityLayer;
    private Transform scavenging;

    [Header("Station")]
    [SerializeField] private Vector3 station;
    private bool hasStation;
    [SerializeReference] private float stationCooldownStart = 10f;
    private float stationCooldownTimer;
    [SerializeField] private float droneStationHeight = 1f;

    private List<DronePassiveModule> passiveModules = new List<DronePassiveModule>();
    private List<DroneActiveModule> activeModules = new List<DroneActiveModule>();
    private List<DroneWeaponModule> weaponModules = new List<DroneWeaponModule>();

    [SerializeField] private List<ModuleType> extraDefaultModules = new List<ModuleType>();
    private List<DroneModule> appliedModules = new List<DroneModule>();
    public List<DroneModule> AppliedModules => appliedModules;

    public bool CanActivateActives
    {
        get
        {
            foreach (DroneActiveModule active in activeModules)
            {
                if (active.CoolingDown) return false;
            }
            return true;
        }
    }

    public Vector2 CooldownTime
    {
        get
        {
            float x = 0;
            float y = 0;
            foreach (DroneActiveModule active in activeModules)
            {
                x += active.CurrentCooldown;
                y += active.CooldownStart;
            }
            return new Vector2(x, y);
        }
    }

    [Header("References")]
    private PlayerDroneController playerDroneController;
    private PlayerMovement playerMovement;
    private Transform player;
    private DroneBasicsModule droneBasics;
    [SerializeField] private DurationBar stationCooldownBar;
    private Collider col;
    public Collider Col => col;

    private void Start()
    {
        // Set References
        player = GameManager._Instance.Player;
        playerDroneController = player.GetComponent<PlayerDroneController>();
        col = GetComponent<SphereCollider>();
        droneBasics = GetComponentInChildren<DroneBasicsModule>();

        // Spawn Station Cooldown Bar
        stationCooldownBar = Instantiate(stationCooldownBar, transform);
    }

    private void Update()
    {
        orbitTimer += Time.deltaTime * OrbitSpeed;
        stationCooldownTimer -= Time.deltaTime;
        switch (CurrentMode)
        {
            case DroneMode.ATTACK:
                HandleFollowModeLogic();
                break;
            case DroneMode.SCAVENGE:
                HandleScavengeModeLogic();
                break;
            case DroneMode.STATION:
                HandleStationModeLogic();
                break;
        }
    }

    // Cycles the drone mode, current order is FOLLOW -> SCAVENGE -> FOLLOW
    public void CycleDroneMode()
    {
        switch (CurrentMode)
        {
            case DroneMode.ATTACK:
                CurrentMode = DroneMode.SCAVENGE;
                OnEnterScavengeMode();
                scavenging = null;
                break;
            case DroneMode.SCAVENGE:
                CurrentMode = DroneMode.ATTACK;
                OnEnterAttackMode();
                break;
            case DroneMode.STATION:
                CurrentMode = DroneMode.ATTACK;
                hasStation = false;
                OnEnterAttackMode();
                break;
        }
    }

    public void OnEnterAttackMode()
    {
        // Enable all attacking types
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.OnEnterAttackMode();
        }
    }

    public void OnEnterScavengeMode()
    {
        // Disable all attacking types
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.OnEnterScavengeMode();
        }
    }

    public void OnEnterStationMode()
    {
        // Disable all attacking types
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.OnEnterAttackMode();
        }
    }

    public int GetNumberOfModules(ModuleCategory type)
    {
        return DroneModule.GetNumModulesOfCategory(type, appliedModules);
    }

    public void SetOrbitTimer(float value)
    {
        orbitTimer = value;
    }

    public void SetRotateAround(Transform target)
    {
        rotateAround = target;
    }

    private void HandleFollowModeLogic()
    {
        RotateAroundTarget();
    }

    private void RotateAroundTarget()
    {
        if (rotateAround == null) return;
        transform.position =
            Vector3.MoveTowards(transform.position, GetOrbitPosition(), Time.deltaTime * MoveSpeed);
    }

    // Clamps the drone's position to be on a circles edge around the given anchor, with some min and max radius
    private Vector3 GetOrbitPosition()
    {
        float radius = 1 * OrbitDistance; // Outer radius
                                          // Find Direction
        float angle = orbitTimer % 360;

        Vector3 position = new Vector3(
            rotateAround.position.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
            rotateAround.position.y,
            rotateAround.position.z + Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
        return position;
    }

    private Collider[] GetUnclaimedScavengeables(Collider[] colliders)
    {
        List<Collider> unclaimedScavengeables = new List<Collider>();
        foreach (Collider col in colliders)
        {
            unclaimedScavengeables.Add(col);
            foreach (DroneController drone in playerDroneController.TrackedDrones)
            {
                if (drone.scavenging == col.transform)
                {
                    unclaimedScavengeables.Remove(col);
                    break;
                }
            }
        }
        return unclaimedScavengeables.ToArray();
    }

    private void FindNewScavengeable()
    {
        // Find new Target
        // Priority; if there are priority scavengeable objects in range, we grab those first
        Collider[] inRange = Physics.OverlapSphere(player.position, ScavengeableSightRange, scavengeablePriorityLayer);
        if (inRange.Length > 0)
        {
            inRange = GetUnclaimedScavengeables(inRange);
            scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            return;
        }

        // Otherwise, we scavenge the normal priority scavengeables
        inRange = Physics.OverlapSphere(player.position, ScavengeableSightRange, scavengeableLayer);
        if (inRange.Length > 0)
        {
            inRange = GetUnclaimedScavengeables(inRange);
            scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            return;
        }
        scavenging = null;
    }

    private void HandleScavengeModeLogic()
    {
        if (scavenging == null || !scavenging.gameObject.activeInHierarchy)
        {
            FindNewScavengeable();
            if (scavenging == null) HandleFollowModeLogic();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, scavenging.position,
                Time.deltaTime * MoveSpeed * ScavengingSpeedMod);
            if (Vector3.Distance(transform.position, scavenging.position) <= ScavengingSpeedMod)
            {
                // "Pick Up" Object
                scavenging.GetComponent<Scavengeable>().OnPickup();

                // No longer scavenging it, will find new object
                FindNewScavengeable();
            }
        }
    }

    private void HandleStationModeLogic()
    {
        if (hasStation)
        {
            // If drone is ref of screen, remove it's station
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
            if (!onScreen)
            {
                hasStation = false;
                return;
            }

            // Move towards station
            transform.position =
                Vector3.MoveTowards(transform.position, station, Time.deltaTime * MoveSpeed);
        }
        else
        {
            HandleFollowModeLogic();
        }
    }

    public void SetStation(Vector3 pos)
    {
        if (stationCooldownTimer > 0) return;
        hasStation = true;
        station = pos + Vector3.one * droneStationHeight;

        stationCooldownTimer = stationCooldownStart;
        stationCooldownBar.SetText("Stationing");
        stationCooldownBar.Set(stationCooldownTimer);
    }

    internal void ResetStationCooldown()
    {
        stationCooldownTimer = 0;
        stationCooldownBar.Cancel();
    }

    public void ResetState()
    {
        // Set to Follow
        CurrentMode = DroneMode.ATTACK;

        // Clear Lists
        passiveModules.Clear();
        activeModules.Clear();
        weaponModules.Clear();

        // Remove all module components from this drone
        foreach (DroneModule module in appliedModules)
        {
            Destroy(module.gameObject);
        }
        appliedModules.Clear();

        // Reset has Station
        hasStation = false;

        // Add Default Modules
        foreach (ModuleType moduleType in GameManager._Instance.DefaultModules)
        {
            GameManager._Instance.AddModule(this, moduleType);
        }

        foreach (ModuleType moduleType in extraDefaultModules)
        {
            GameManager._Instance.AddModule(this, moduleType);
        }
    }

    // Takes in a prefab and returns a new instance
    public DroneModule AddModule(DroneModule module)
    {
        // Instantiate new instance of passed in module prefab
        module = Instantiate(module, transform);
        appliedModules.Add(module);
        // Determine which list to add it to
        switch (module.Category)
        {
            case ModuleCategory.ACTIVE:
                AddActiveModule((DroneActiveModule)module);
                break;
            case ModuleCategory.PASSIVE:
                if (module.Type == ModuleType.HELP_PLAYER_MOVEMENT)
                {
                    // Specific case, if the module is of type "HelpPlayerMovementModule", we need to set the corresponding variable in the player
                    if (playerMovement == null)
                        playerMovement = GameManager._Instance.Player.GetComponent<PlayerMovement>();
                    playerMovement.SetHelpPlayerMovementModule((HelpPlayerMovementModule)module);
                }
                AddPassiveModule((DronePassiveModule)module);
                break;
            case ModuleCategory.WEAPON:
                AddWeaponModule((DroneWeaponModule)module);
                break;
        }
        return module;
    }

    private void AddPassiveModule(DronePassiveModule type)
    {
        // Add the module
        passiveModules.Add(type);
    }

    private void AddActiveModule(DroneActiveModule type)
    {
        // Add the module
        activeModules.Add(type);
    }

    private void AddWeaponModule(DroneWeaponModule type)
    {
        // Add the module
        weaponModules.Add(type);

        // It can begin attacking asap if not in scavenge mode
        if (CurrentMode != DroneMode.SCAVENGE)
            type.OnAdd();
    }

    public void ActivateActives()
    {
        foreach (DroneActiveModule active in activeModules)
        {
            active.Activate();
        }
    }
    public void ResetActiveCooldown()
    {
        foreach (DroneActiveModule active in activeModules)
        {
            active.ResetCooldown();
        }
    }
}