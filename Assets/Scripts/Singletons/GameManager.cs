using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }

    [SerializeField] private List<ModuleType> addedDroneWeaponModules = new List<ModuleType>();
    public List<ModuleType> DroneWeaponModules => addedDroneWeaponModules;
    [SerializeField] private int startingPlayerResource;
    [SerializeField] private int currentPlayerResource;
    public int CurrentPlayerResource => currentPlayerResource;

    [Header("References")]
    public Transform Player;
    [SerializeField] private PlayerDroneOrbitController playerDroneController;
    [SerializeField] private DroneController dronePrefab;
    [SerializeField] private float droneSpawnHeight;

    private void Start()
    {
        AddResource(startingPlayerResource);
        ResetScriptableObjects();
    }

    /*
    private int playerXPGoal;
    [SerializeField] private Bar XPBar;
    */

    public void AddResource(int value)
    {
        // Add XP
        currentPlayerResource += value;
    }

    public void SpawnAndAddDroneToOrbit()
    {
        // Don't add drone if reached cap
        if (!playerDroneController.UnderDroneLimit) return;

        // Spawn Drone
        DroneController spawnedDrone = Instantiate(dronePrefab,
            Player.position + new Vector3(0, droneSpawnHeight, 0), Quaternion.identity);

        // Add new drone to orbit
        playerDroneController.AddDroneToOrbit(spawnedDrone);

        // Add Weapon Modules
        foreach (ModuleType type in addedDroneWeaponModules)
        {
            AddModuleToDrone(type, spawnedDrone.gameObject);
        }
    }

    public bool AddModule(ModuleType type, int cost, bool free)
    {
        if (free)
        {
            addedDroneWeaponModules.Add(type);
            // Add new module to all tracked drones
            AddModuleToDrones(type);
            return true;
        }
        else
        {
            if (currentPlayerResource < cost) return false;

            addedDroneWeaponModules.Add(type);
            currentPlayerResource -= cost;

            // Add new module to all tracked drones
            AddModuleToDrones(type);

            // Will need to actually handle the logic of apply components here

            return true;
        }
    }

    private void AddModuleToDrones(ModuleType type)
    {
        List<DroneController> trackedDrones = playerDroneController.TrackedDrones;
        foreach (DroneController drone in trackedDrones)
        {
            AddModuleToDrone(type, drone.gameObject);
        }
    }

    private DroneModule AddModuleToDrone(ModuleType type, GameObject drone)
    {
        switch (type)
        {
            case ModuleType.BASIC_TURRET:
                DroneGunModule basicTurretModule = drone.AddComponent<DroneTurretModule>();
                basicTurretModule.Set(automaticGun);
                return basicTurretModule;
            case ModuleType.BURST_FIRE_TURRET:
                DroneGunModule burstFireModule = drone.AddComponent<DroneTurretModule>();
                burstFireModule.Set(burstFireGun);
                return burstFireModule;
            case ModuleType.TESLA_COIL:
                DroneTeslaModule chainLightningModule = drone.AddComponent<DroneTeslaModule>();
                chainLightningModule.Set();
                return chainLightningModule;
            case ModuleType.EXPLOSIVE_DROPPER_SHELL_MORTAR:
                DroneGunModule explosiveShellDropperModule = drone.AddComponent<DroneExplosiveShellDropperMortarModule>();
                explosiveShellDropperModule.Set(explosiveShellDropperMortarGun);
                return explosiveShellDropperModule;
            case ModuleType.EXPLOSIVE_SHELL_MORTAR:
                DroneGunModule explosiveShellModule = drone.AddComponent<DroneMortarModule>();
                explosiveShellModule.Set(explosiveShellMortarGun);
                return explosiveShellModule;
            case ModuleType.TOXIC_SHELL_MORTAR:
                DroneGunModule toxicShellModule = drone.AddComponent<DroneMortarModule>();
                toxicShellModule.Set(toxicShellMortarGun);
                return toxicShellModule;
            case ModuleType.SNIPER_TURRET:
                DroneGunModule sniperModule = drone.AddComponent<DroneSniperModule>();
                sniperModule.Set(sniperGun);
                return sniperModule;
            case ModuleType.DRONE_CONTACT_DAMAGE:
                DroneContactDamageModule contactDamageModule = drone.AddComponent<DroneContactDamageModule>();
                return contactDamageModule;
            case ModuleType.DRONE_BLOCK_BULLETS:
                DroneBlockBulletsModule blockBulletsModule = drone.AddComponent<DroneBlockBulletsModule>();
                return blockBulletsModule;
            default:
                return null;

        }
    }

    [Header("Guns")]
    [SerializeField] private Gun automaticGun;
    [SerializeField] private Gun burstFireGun;
    [SerializeField] private Gun sniperGun;
    [SerializeField] private MortarGun explosiveShellMortarGun;
    [SerializeField] private MortarGun explosiveShellDropperMortarGun;
    [SerializeField] private MortarGun toxicShellMortarGun;

    private void ResetScriptableObjects()
    {
        StatModifier[] statModifiers = Resources.LoadAll<StatModifier>("");
        foreach (StatModifier statModifier in statModifiers)
        {
            // Debug.Log("Resetting: " + statModifier);
            statModifier.Reset();
        }

        UpgradeNode[] upgradeNodes = Resources.LoadAll<UpgradeNode>("");
        foreach (UpgradeNode node in upgradeNodes)
        {
            // Debug.Log("Resetting: " + node);
            node.Reset();
        }
    }
}

