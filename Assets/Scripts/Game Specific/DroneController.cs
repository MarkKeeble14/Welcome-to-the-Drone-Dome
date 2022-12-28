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
    [SerializeField] private float scavengeableSightRange;
    [SerializeField] private float scavengingGrabRange;
    [SerializeField] private float scavengingSpeedMod;
    private Transform scavenging;

    [Header("References")]
    public Collider Col;
    private Transform player;
    private PlayerDroneOrbitController playerDroneOrbitController;

    private void Start()
    {
        // Set References
        player = GameManager._Instance.Player;
        playerDroneOrbitController = player.GetComponent<PlayerDroneOrbitController>();
    }

    // Cycles the drone mode, current order is FOLLOW -> SCAVENGE -> FOLLOW
    public void CycleDroneMode()
    {
        switch (CurrentMode)
        {
            case DroneMode.FOLLOW:
                CurrentMode = DroneMode.SCAVENGE;
                DisableAttackModules();
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
        foreach (DroneAttackModule attackModule in GetComponents<DroneAttackModule>())
        {
            attackModule.StartAttack();
        }
    }

    public void DisableAttackModules()
    {
        // Disable all attacking types
        foreach (DroneAttackModule attackModule in GetComponents<DroneAttackModule>())
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
            Collider[] inRange = Physics.OverlapSphere(player.position, scavengeableSightRange, scavengeableLayer);
            inRange = GetUnclaimedScavengeables(inRange);
            if (inRange.Length > 0)
            {
                scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            }
            else
            {
                HandleFollowModeLogic();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, scavenging.position) < scavengingGrabRange)
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

    private Collider[] GetUnclaimedScavengeables(Collider[] colliders)
    {
        List<Collider> unclaimedScavengeables = new List<Collider>();
        foreach (Collider col in colliders)
        {
            unclaimedScavengeables.Add(col);
            foreach (DroneController drone in playerDroneOrbitController.TrackedDrones)
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