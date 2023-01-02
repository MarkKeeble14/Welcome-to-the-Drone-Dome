using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public DroneMode CurrentMode;

    public bool AvailableForUse = true;

    public Transform Follow;

    [SerializeField] private float moveSpeed;

    [SerializeField] private float shoveStrength;
    public float ShoveStrength { get { return shoveStrength; } }
    [SerializeField] private int shoveLimit;
    public int ShoveLimit { get { return shoveLimit; } }

    [SerializeField] private LayerMask scavengeableLayer;
    [SerializeField] private LayerMask scavengeablePriorityLayer;
    [SerializeField] private float scavengeableSightRange;
    [SerializeField] private float scavengingGrabRange;
    [SerializeField] private float scavengingSpeedMod;
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

    public void ResetActiveCooldown()
    {
        foreach (DroneActiveModule active in activeModules)
        {
            active.ResetCooldown();
        }
    }

    [Header("References")]
    public Collider Col;
    private Transform player;
    private PlayerDroneController playerDroneController;

    public void AddPassiveModule(DronePassiveModule type)
    {
        passiveModules.Add(type);
        if (!appliedModules.Contains(type))
            appliedModules.Add(type);
    }

    public void AddActiveModule(DroneActiveModule type)
    {
        activeModules.Add(type);
        if (!appliedModules.Contains(type))
            appliedModules.Add(type);
    }

    public void AddWeaponModule(DroneWeaponModule type)
    {
        weaponModules.Add(type);
        if (!appliedModules.Contains(type))
            appliedModules.Add(type);
    }

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
            case DroneMode.FOLLOW:
                CurrentMode = DroneMode.SCAVENGE;
                DisableAttackModules();
                scavenging = null;
                break;
            case DroneMode.SCAVENGE:
                CurrentMode = DroneMode.FOLLOW;
                EnableAttackModules();
                break;
        }
    }

    public void EnableAttackModules()
    {
        // Enable all attacking types
        foreach (DroneWeaponModule attackModule in GetComponents<DroneWeaponModule>())
        {
            attackModule.StartAttack();
        }
    }

    public void DisableAttackModules()
    {
        // Disable all attacking types
        foreach (DroneWeaponModule attackModule in GetComponents<DroneWeaponModule>())
        {
            attackModule.StopAttack();
        }
    }

    private void Update()
    {
        switch (CurrentMode)
        {
            case DroneMode.FOLLOW:
                HandleFollowModeLogic();
                break;
            case DroneMode.SCAVENGE:
                HandleScavengeModeLogic();
                break;
        }
    }

    private void HandleFollowModeLogic()
    {
        if (Follow == null) return;

        transform.position =
            Vector3.MoveTowards(transform.position, Follow.position, Time.deltaTime * moveSpeed);
    }

    private void HandleScavengeModeLogic()
    {
        if (scavenging == null)
        {
            // Priority; if there are priority scavengeable objects in range, we grab those first
            Collider[] inRange = Physics.OverlapSphere(player.position, scavengeableSightRange, scavengeablePriorityLayer);
            if (inRange.Length > 0)
            {
                inRange = GetUnclaimedScavengeables(inRange);
                scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
                return;
            }

            // Otherwise, we scavenge the normal priority scavengeables
            inRange = Physics.OverlapSphere(player.position, scavengeableSightRange, scavengeableLayer);
            if (inRange.Length > 0)
            {
                inRange = GetUnclaimedScavengeables(inRange);
                scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            }
            else
            {
                HandleFollowModeLogic();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, scavenging.position) <= scavengingGrabRange)
            {
                // "Pick Up" Object
                scavenging.GetComponent<Scavengeable>().OnPickup();

                // No longer scavenging it, will find new object
                scavenging = null;
            }
            else
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, scavenging.position, Time.deltaTime * moveSpeed * scavengingSpeedMod);
            }
        }
    }

    public void ResetState()
    {
        // Set to Follow
        CurrentMode = DroneMode.FOLLOW;

        // Clear Lists
        passiveModules.Clear();
        activeModules.Clear();
        weaponModules.Clear();

        // Remove all module components from this drone
        foreach (DroneModule module in appliedModules)
        {
            Destroy(module);
        }
        appliedModules.Clear();
    }

    internal void AddModule(DroneModule module)
    {
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
    }

    internal void ActivateActives()
    {
        foreach (DroneActiveModule active in activeModules)
        {
            active.Activate();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, scavengeableSightRange);
    }
}