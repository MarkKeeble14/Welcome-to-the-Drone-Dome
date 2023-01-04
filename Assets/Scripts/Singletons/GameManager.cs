using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [Header("Drones")]
    [SerializeField]
    private SerializableDictionary<ModuleType, DroneModuleInfo> moduleTypeInfo
        = new SerializableDictionary<ModuleType, DroneModuleInfo>();
    public List<ModuleType> AllModules => moduleTypeInfo.KeysWhereValueMeetsCondition(moduleInfo => !moduleInfo.Unobtainable);
    [SerializeField] private int numDronesToStart = 1;
    [SerializeField] private int activesPerDrone = 1;
    public int ActivesPerDrone => activesPerDrone;
    [SerializeField] private int passivesPerDrone = 1;
    public int PassivesPerDrone => passivesPerDrone;
    [SerializeField] private int weaponsPerDrone = 3;
    public int WeaponsPerDrone => weaponsPerDrone;
    public float DroneSpawnHeight => targetCameraZoom.y + 1;

    [Header("Guns")]
    [SerializeField] private Gun automaticGun;
    [SerializeField] private Gun burstFireGun;
    [SerializeField] private Gun sniperGun;
    [SerializeField] private MortarGun explosiveShellMortarGun;
    [SerializeField] private MortarGun explosiveShellDropperMortarGun;
    [SerializeField] private MortarGun toxicShellMortarGun;
    [SerializeField] private MortarGun thumperMortarGun;

    [Header("Level")]
    [SerializeField] private string[] levelNames;
    private int levelIndex = 0;
    private bool loadingLevel;
    private bool restartingGame;
    [SerializeField] private bool shouldLoadOnStart;
    [SerializeField] private int loadOnStart;
    [SerializeField] private float maxWaitTime = 5f;

    public bool OnLastLevel => levelIndex == levelNames.Length - 1;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera cinemachineVCam;
    private CinemachineTransposer cinemachineTransposer;
    public float CameraZoomSpeed = 1f;
    public Vector3 defaultCameraOffset = new Vector3(0, 12f, 0);

    [Header("References")]
    public Transform Player;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private DroneController dronePrefab;
    [SerializeField] private ShowSelectedDronesModulesDisplay showSelectedDronesModules;
    private Vector3 targetCameraZoom;

    private void Start()
    {
        // Debug.Log("Game Manager Start Called");
        // Manage Game Stuff
        ResetScriptableObjects();

        // Set camera offset
        cinemachineTransposer = cinemachineVCam.GetCinemachineComponent<CinemachineTransposer>();
        cinemachineTransposer.m_FollowOffset = defaultCameraOffset;
        targetCameraZoom = defaultCameraOffset;

        // Load different scene on start if required
        if (shouldLoadOnStart)
        {
            levelIndex = loadOnStart;
            SceneManager.LoadScene(levelNames[levelIndex]);
        }

        // Set the correct drones display
        UIManager._Instance.SetCurrentDronesDisplayForMenu();
        // Give player starting drones
        for (int i = 0; i < numDronesToStart; i++)
        {
            SpawnAndAddDroneToOrbit();
        }
    }

    public void OnPlayerDie()
    {
        Debug.Log("You Lose!");
        UIManager._Instance.OpenLoseScreen();
    }

    public void Win()
    {
        Debug.Log("You Win!");
        UIManager._Instance.OpenWinScreen();
    }

    public void SpawnAndAddDroneToOrbit()
    {
        // Don't add drone if reached cap
        if (!playerDroneController.UnderDroneLimit) return;

        // Spawn Drone
        DroneController spawnedDrone = Instantiate(dronePrefab,
            Player.position + new Vector3(0, DroneSpawnHeight, 0), Quaternion.identity);
        GameElements._Instance.AddedObjects.Add(spawnedDrone.gameObject);

        // Add new drone to orbit
        playerDroneController.AddDroneToOrbit(spawnedDrone);
    }

    public void LoadNextLevel()
    {
        // Debug.Log("Attempting to Load Next Level");
        if (loadingLevel) return;
        loadingLevel = true;
        StartCoroutine(LoadNextLevelSequence());
    }

    private IEnumerator LoadNextLevelSequence()
    {
        // Collect all remaining resource
        List<AutoCollectScavengeable> collectedAutoCollectable = new List<AutoCollectScavengeable>();
        AutoCollectScavengeable[] remainingAutoCollectable = FindObjectsOfType<AutoCollectScavengeable>();
        foreach (AutoCollectScavengeable autoCollectable in remainingAutoCollectable)
        {
            autoCollectable.AutoCollect(() =>
            {
                collectedAutoCollectable.Add(autoCollectable);
            }
            );
        }

        float t = 0;
        while (t < maxWaitTime && collectedAutoCollectable.Count != remainingAutoCollectable.Length)
        {
            t += Time.deltaTime;

            yield return null;
        }

        // Load Next Level
        // Debug.Log("Loading Next Level");
        TransitionManager._Instance.FadeOut(() =>
        {
            LoadLevel(true);
        });
    }

    private void LoadLevel(bool incrementIndex)
    {
        // Functions to call on loading a new level
        // Open in game UI
        UIManager._Instance.OpenInGameUI();

        // Load the level scene
        if (incrementIndex) levelIndex++;
        SceneManager.LoadScene(levelNames[levelIndex]);

        // Reset active cooldowns
        playerDroneController.ResetDroneActiveCooldowns();
        playerDroneController.ResetDronePositions();
        // Reset player position
        Player.position = Vector3.zero;
        // Reset player dash cooldown
        playerMovement.ResetDashCooldown();
        // Reset camera position
        cinemachineVCam.transform.position = new Vector3(0, cinemachineVCam.transform.position.y, 0);
        // No longer loading level
        loadingLevel = false;

        // Start Fade in transition
        TransitionManager._Instance.FadeIn();
    }

    public void StartGame()
    {
        // Load Next Level
        // Debug.Log("Loading Next Level");
        TransitionManager._Instance.FadeOut(() =>
        {
            // We must reset anything that may have been altered in the main menu scene here
            LoadLevel(false);
            // Reset Stats
            ResetScriptableObjects();
            // Reset Drones
            ResetPlayerDrones();
            // Reset Collectables
            ShopManager._Instance.ResetCollectables();
        });
    }

    public void ResetPlayerDrones()
    {
        // Reset Drones
        playerDroneController.ResetDrones();
    }

    public void RestartGame()
    {
        if (restartingGame) return;
        Debug.Log("Restarting Game");
        restartingGame = true;
        TransitionManager._Instance.FadeOut(() =>
        {
            UIManager._Instance.SetCurrentDronesDisplayForMenu();
            SceneManager.LoadScene("MainMenu");
        });
    }

    public ModuleCategory GetModuleCategory(ModuleType type)
    {
        return moduleTypeInfo.GetEntry(type).Value.Category;
    }

    public Color GetModuleColor(ModuleType type)
    {
        return moduleTypeInfo.GetEntry(type).Value.Color;
    }

    public DroneModule TryPurchaseModule(ModuleType type, int cost, bool free)
    {
        // Check Player has Selected
        DroneController drone = playerDroneController.SelectedDrone;
        if (drone == null) return null;

        // Check if drone is accepting more modules of that type; if so, add it
        switch (GetModuleCategory(type))
        {
            case ModuleCategory.ACTIVE:
                if (drone.NumActives >= activesPerDrone) return null;
                break;
            case ModuleCategory.PASSIVE:
                if (drone.NumPassives >= passivesPerDrone) return null;
                break;
            case ModuleCategory.WEAPON:
                if (drone.NumWeapons >= weaponsPerDrone) return null;
                break;
        }

        // Check Cost; if free, don't worry about cost/payment
        if (free)
        {
            ShopManager._Instance.UseFreePurchase();
        }
        else // Must have enough money, also payment
        {
            if (ShopManager._Instance.CurrentPlayerResource < cost) return null;
            ShopManager._Instance.AlterResource(-cost);
        }

        // Add new selection to show selected drone modules
        showSelectedDronesModules.AddModule(type);

        // Actually add module
        return AddModule(drone, type);
    }

    public DroneModule AddModule(DroneController drone, ModuleType type)
    {
        switch (type)
        {
            case ModuleType.BASIC_TURRET:
                DroneGunModule basicTurretModule = drone.gameObject.AddComponent<DroneTurretModule>();
                basicTurretModule.Set(automaticGun);
                drone.AddModule(basicTurretModule);
                return basicTurretModule;
            case ModuleType.BURST_FIRE_TURRET:
                DroneGunModule burstFireModule = drone.gameObject.AddComponent<DroneBurstFireTurretModule>();
                burstFireModule.Set(burstFireGun);
                drone.AddModule(burstFireModule);
                return burstFireModule;
            case ModuleType.TESLA_COIL:
                DroneTeslaModule chainLightningModule = drone.gameObject.AddComponent<DroneTeslaModule>();
                chainLightningModule.Set();
                drone.AddModule(chainLightningModule);
                return chainLightningModule;
            case ModuleType.EXPLOSIVE_DROPPER_SHELL_MORTAR:
                DroneGunModule explosiveShellDropperModule = drone.gameObject.AddComponent<DroneExplosiveShellDropperMortarModule>();
                explosiveShellDropperModule.Set(explosiveShellDropperMortarGun);
                drone.AddModule(explosiveShellDropperModule);
                return explosiveShellDropperModule;
            case ModuleType.EXPLOSIVE_SHELL_MORTAR:
                DroneGunModule explosiveShellModule = drone.gameObject.AddComponent<DroneExplosiveShellMortarModule>();
                explosiveShellModule.Set(explosiveShellMortarGun);
                drone.AddModule(explosiveShellModule);
                return explosiveShellModule;
            case ModuleType.TOXIC_SHELL_MORTAR:
                DroneGunModule toxicShellModule = drone.gameObject.AddComponent<DroneToxicShellMortarModule>();
                toxicShellModule.Set(toxicShellMortarGun);
                drone.AddModule(toxicShellModule);
                return toxicShellModule;
            case ModuleType.SNIPER_TURRET:
                DroneGunModule sniperModule = drone.gameObject.AddComponent<DroneSniperModule>();
                sniperModule.Set(sniperGun);
                drone.AddModule(sniperModule);
                return sniperModule;
            case ModuleType.DRONE_CONTACT_DAMAGE:
                DroneContactDamageModule contactDamageModule = drone.gameObject.AddComponent<DroneContactDamageModule>();
                drone.AddModule(contactDamageModule);
                return contactDamageModule;
            case ModuleType.DRONE_BLOCK_BULLETS:
                DroneBlockBulletsModule blockBulletsModule = drone.gameObject.AddComponent<DroneBlockBulletsModule>();
                drone.AddModule(blockBulletsModule);
                return blockBulletsModule;
            case ModuleType.THUMPER_MORTAR:
                DroneGunModule thumperModule = drone.gameObject.AddComponent<DroneThumperModule>();
                drone.AddModule(thumperModule);
                thumperModule.Set(thumperMortarGun);
                return thumperModule;
            case ModuleType.LASER_ACTIVE:
                DroneLaserAreaActiveModule laserAreaActiveModule = drone.gameObject.AddComponent<DroneLaserAreaActiveModule>();
                drone.AddModule(laserAreaActiveModule);
                return laserAreaActiveModule;
            default:
                return null;

        }
    }

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

    public void IncreaseCameraZoom(float bossZoomOut)
    {
        targetCameraZoom = targetCameraZoom + new Vector3(0, bossZoomOut, 0);
    }

    public void DecreaseCameraZoom(float bossZoomOut)
    {
        targetCameraZoom = targetCameraZoom - new Vector3(0, bossZoomOut, 0);
    }

    private void Update()
    {
        // Zoom in/out as needed
        cinemachineTransposer.m_FollowOffset
                = Vector3.MoveTowards(cinemachineTransposer.m_FollowOffset, targetCameraZoom,
                Time.deltaTime * CameraZoomSpeed);
    }

    public void HealPlayer(float healAmount)
    {
        playerHealth.Heal(healAmount, true);
    }
}
