﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public DroneMode CurrentMode;

    public bool AvailableForUse = true;

    public Transform Follow;
    [SerializeField] private Vector3 station;
    private bool hasStation;
    [SerializeReference] private float stationCooldownStart = 10f;
    private float stationCooldownTimer;
    [SerializeField] private float droneStationHeight = 1f;

    [SerializeField] private StatModifier moveSpeed;
    public float MoveSpeed => moveSpeed.Value;

    [SerializeField] private StatModifier shoveStrength;
    public float ShoveStrength { get { return shoveStrength.Value; } }
    [SerializeField] private int shoveLimit;
    public int ShoveLimit { get { return shoveLimit; } }

    [SerializeField] private LayerMask scavengeableLayer;
    [SerializeField] private LayerMask scavengeablePriorityLayer;
    [SerializeField] private StatModifier scavengeableSightRange;
    [SerializeField] private StatModifier scavengingGrabRange;
    [SerializeField] private StatModifier scavengingSpeedMod;
    private Transform scavenging;

    private List<DronePassiveModule> passiveModules = new List<DronePassiveModule>();
    public int NumPassives => passiveModules.Count;
    private List<DroneActiveModule> activeModules = new List<DroneActiveModule>();
    public int NumActives => activeModules.Count;
    private List<DroneWeaponModule> weaponModules = new List<DroneWeaponModule>();
    public int NumWeapons => weaponModules.Count;
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
    public Collider Col;
    [SerializeField] private DurationBar stationCooldown;
    private Transform player;
    private PlayerDroneController playerDroneController;

    private void Start()
    {
        // Set References
        player = GameManager._Instance.Player;
        playerDroneController = player.GetComponent<PlayerDroneController>();
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
                CurrentMode = DroneMode.STATION;
                OnEnterStationMode();
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


    private void HandleFollowModeLogic()
    {
        if (Follow == null) return;

        transform.position =
            Vector3.MoveTowards(transform.position, Follow.position, Time.deltaTime * moveSpeed.Value);
    }

    private void FindNewScavengeable()
    {
        // Find new Target
        // Priority; if there are priority scavengeable objects in range, we grab those first
        Collider[] inRange = Physics.OverlapSphere(player.position, scavengeableSightRange.Value, scavengeablePriorityLayer);
        if (inRange.Length > 0)
        {
            inRange = GetUnclaimedScavengeables(inRange);
            scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            return;
        }

        // Otherwise, we scavenge the normal priority scavengeables
        inRange = Physics.OverlapSphere(player.position, scavengeableSightRange.Value, scavengeableLayer);
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
                Time.deltaTime * moveSpeed.Value * scavengingSpeedMod.Value);
            if (Vector3.Distance(transform.position, scavenging.position) <= scavengingGrabRange.Value)
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
            // If drone is out of screen, remove it's station
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
            if (!onScreen)
            {
                hasStation = false;
                return;
            }

            // Move towards station
            transform.position =
                Vector3.MoveTowards(transform.position, station, Time.deltaTime * moveSpeed.Value);
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
        stationCooldown.Set(stationCooldownTimer);
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

        // Add Built in Turret
        GameManager._Instance.AddModule(this, ModuleType.DEFAULT_TURRET);
    }

    // Takes in a prefab and returns a new instance
    public DroneModule AddModule(DroneModule module)
    {
        // Instantiate new instance of passed in module prefab
        module = Instantiate(module, transform);

        // Determine which list to add it to
        switch (module.Category)
        {
            case ModuleCategory.ACTIVE:
                AddActiveModule((DroneActiveModule)module);
                break;
            case ModuleCategory.PASSIVE:
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
        passiveModules.Add(type);
        if (!appliedModules.Contains(type))
            appliedModules.Add(type);
    }

    private void AddActiveModule(DroneActiveModule type)
    {
        activeModules.Add(type);
        if (!appliedModules.Contains(type))
            appliedModules.Add(type);
    }

    private void AddWeaponModule(DroneWeaponModule type)
    {
        // Add the module
        weaponModules.Add(type);

        // Add to master list
        if (!appliedModules.Contains(type))
            appliedModules.Add(type);

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

    private void Update()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, scavengeableSightRange.Value);
    }
}