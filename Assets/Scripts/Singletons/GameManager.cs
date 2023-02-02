using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
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
    [SerializeField]
    private SerializableDictionary<UpgradeTreeRelation, ModuleType> upgradeTreeRelationInfo
    = new SerializableDictionary<UpgradeTreeRelation, ModuleType>();
    [SerializeField]
    private SerializableDictionary<UpgradeTreeRelation, UpgradeTreeDisplayInfo> upgradeTreeDisplayInfo = new SerializableDictionary<UpgradeTreeRelation, UpgradeTreeDisplayInfo>();
    public SerializableDictionary<ModuleType, DroneModuleInfo> ModuleTypeInfo => moduleTypeInfo;
    public List<ModuleType> AllModules => moduleTypeInfo.KeysWhereValueMeetsCondition(moduleInfo => !moduleInfo.Unobtainable);
    [SerializeField] private List<ModuleType> defaultModules = new List<ModuleType>();
    public List<ModuleType> DefaultModules => defaultModules;
    [SerializeField] private int numDronesToStart = 1;
    public float DroneSpawnHeight => targetCameraZoom.y + 1;

    [SerializeField] private float wavesCompletedLinearLimit = 6f;

    [Header("Level")]
    [SerializeField] private string[] levelNames;
    [SerializeField] private ArenaManager[] levels;
    private ArenaManager currentLevel;
    private int levelIndex = 0;
    public int LevelIndex => levelIndex;
    private bool loadingLevel;
    private bool restartingGame;
    [SerializeField] private bool shouldLoadOnStart;
    [SerializeField] private int loadOnStart;

    public bool OnLastLevel => levelIndex == levelNames.Length - 1;
    public bool OnMainMenu => levelIndex == 0 && currentLevel == null;
    public int OnLoop { get; private set; }

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera cinemachineVCam;
    private CinemachineTransposer cinemachineTransposer;
    public float CameraZoomSpeed = 1f;
    public Vector3 defaultCameraOffset = new Vector3(0, 12f, 0);

    [Header("References")]
    public Transform Player;
    [SerializeField] private PlayerDroneController playerDroneController;
    public PlayerDroneController PlayerDroneController => playerDroneController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private DroneController dronePrefab;
    [SerializeField] private LevelAndLoopStatMap perLevelEnemyStatMap;
    public LevelAndLoopStatMap PerLevelEnemyStatMap => perLevelEnemyStatMap;
    public EnemyStatMap EnemyStatMap => perLevelEnemyStatMap.Current;

    private Vector3 targetCameraZoom;
    [SerializeField] private TextMeshProUGUI arenaShopHelperText;

    private int enemiesKilled;
    public int EnemiesKilled => enemiesKilled;
    public int ArenasCleared { get; set; }
    public static float _BaseHeight = .5f;

    [SerializeField] private GameObject mainMenuEnvironment;
    public void IncrementEnemiesKilled()
    {
        enemiesKilled++;
    }

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

        // Music
        AudioManager._Instance.StartLevelMusic();
    }

    public float GetCreditBonus(int wavesCompleted)
    {
        if (wavesCompleted < wavesCompletedLinearLimit)
        {
            return wavesCompleted + 1;
        }
        else
        {
            return (wavesCompleted / 7.5f) * Mathf.Pow(wavesCompleted, 0.1f) + wavesCompletedLinearLimit + 1f;
        }
    }

    public void OnPlayerDie()
    {
        UIManager._Instance.OpenLoseScreen();
    }

    public void Win()
    {
        UIManager._Instance.OpenWinScreen();
    }

    public void Loop()
    {
        // Debug.Log("Attempting to Loop");
        if (loadingLevel) return;
        loadingLevel = true;

        // Scale Enemies
        perLevelEnemyStatMap.OnLoop();

        // Call Loop Sequence
        StartCoroutine(LoopSequence());
    }

    public void SpawnAndAddDroneToOrbit()
    {
        // Don't add drone if reached cap
        if (!playerDroneController.UnderDroneLimit) return;

        // Spawn Drone
        DroneController spawnedDrone = Instantiate(dronePrefab,
            Player.position + new Vector3(0, DroneSpawnHeight, 0), Quaternion.identity);
        GameElements._Instance.AddedObjects.Add(spawnedDrone.gameObject);
        spawnedDrone.ResetState();

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
        yield return StartCoroutine(PreLoadNextLevel());

        // Load Next Level
        // Debug.Log("Loading Next Level");
        TransitionManager._Instance.FadeOut(() =>
        {
            LoadLevel(true);
        });
    }

    private IEnumerator LoopSequence()
    {
        yield return StartCoroutine(PreLoadNextLevel());

        // Load Next Level
        // Debug.Log("Loading Next Level");
        TransitionManager._Instance.FadeOut(() =>
        {
            levelIndex = 0;
            LoadLevel(false);
        });
    }

    private IEnumerator PreLoadNextLevel()
    {
        // Collect all remaining autocollectables
        int numCollected = 0;
        AutoCollectScavengeable[] remainingAutoCollectable = FindObjectsOfType<AutoCollectScavengeable>();
        foreach (AutoCollectScavengeable autoCollectable in remainingAutoCollectable)
        {
            autoCollectable.AutoCollect(
            () =>
            {
                numCollected++;
            });
        }

        yield return new WaitUntil(() => numCollected == remainingAutoCollectable.Length);
    }

    private void LoadLevel(bool incrementIndex)
    {
        // Functions to call on loading a new level
        // Open in game UI
        UIManager._Instance.OpenInGameUI();

        // Load the level scene
        if (incrementIndex)
        {
            // Increment the level index
            levelIndex++;
        }
        // Go next enemy stat map
        perLevelEnemyStatMap.SetIndex(levelIndex);

        // Load Scene
        SceneManager.LoadScene(levelNames[levelIndex]);

        // Reset Upgrade Point Cost on a Per Level Basis
        ShopManager._Instance.ResetUpgradePointCost(incrementIndex);

        // Spawn Arena Manager
        if (currentLevel != null)
            Destroy(currentLevel.gameObject);
        currentLevel = Instantiate(levels[levelIndex], transform);

        // Reset drones per stage
        playerDroneController.ResetDroneStationCooldowns();
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

            // Destroy the main menu environment
            Destroy(mainMenuEnvironment);

            // Reset Stats
            ResetScriptableObjects();
            // Reset Drones
            ResetPlayerDrones();

            // Release all scavengeables back to their respective pool
            Scavengeable[] scavengeables = FindObjectsOfType<Scavengeable>();
            foreach (Scavengeable scavengeable in scavengeables)
            {
                scavengeable.ReleaseToPool();
            }

            // Reset Collectables
            ShopManager._Instance.ResetCollectables();
            ShopManager._Instance.AddRandomModulesToAvailableModules();
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
        // Debug.Log("Restarting Game");
        restartingGame = true;
        TransitionManager._Instance.FadeOut(() =>
        {
            UIManager._Instance.SetCurrentDronesDisplayForMenu();
            SceneManager.LoadScene("MainMenu");
        });
    }

    public DroneModuleInfo GetModuleInfo(ModuleType type)
    {
        return moduleTypeInfo.GetEntry(type).Value;
    }

    public ModuleCategory GetModuleCategory(ModuleType type)
    {
        return GetModuleInfo(type).Category;
    }

    public Color GetModuleColor(ModuleType type)
    {
        return GetModuleInfo(type).Color;
    }

    public Sprite GetModuleSprite(ModuleType type)
    {
        return GetModuleInfo(type).Sprite;
    }

    public UpgradeTreeDisplayInfo GetOtherInfo(UpgradeTreeRelation upgradeTreeRelation)
    {
        // Debug.Log(upgradeTreeRelation);
        return upgradeTreeDisplayInfo.GetEntry(upgradeTreeRelation).Value;
    }

    public UpgradeTreeDisplayInfo GetUpgradeTreeDisplayInfo(UpgradeTreeRelation type)
    {
        if (upgradeTreeRelationInfo.ContainsKey(type))
        {
            ModuleType moduleType = upgradeTreeRelationInfo.GetEntry(type).Value;
            return new UpgradeTreeDisplayInfo(GetModuleColor(moduleType), EnumToStringHelper.GetStringValue(moduleType), GetModuleSprite(moduleType));
        }
        else if (upgradeTreeDisplayInfo.ContainsKey(type))
        {
            return upgradeTreeDisplayInfo.GetEntry(type).Value;
        }
        else
        {
            return null;
        }
    }

    public DroneModule TryAddModule(ModuleType type, int cost, bool free)
    {
        // Check Player has Selected
        DroneController drone = playerDroneController.SelectedDrone;
        if (drone == null)
        {
            arenaShopHelperText.gameObject.SetActive(true);
            arenaShopHelperText.text = "You Must First Select a Drone to Purchase a Module";
            return null;
        }

        // Check if drone is accepting more modules of that type; if so, add it
        switch (GetModuleCategory(type))
        {
            case ModuleCategory.ACTIVE:
                if (drone.GetNumberOfModules(ModuleCategory.ACTIVE) >= drone.ActivesPerDrone)
                {
                    arenaShopHelperText.gameObject.SetActive(true);
                    arenaShopHelperText.text = "Unable to Add More Active Modules to this Drone";
                    return null;
                }
                break;
            case ModuleCategory.PASSIVE:
                if (drone.GetNumberOfModules(ModuleCategory.PASSIVE) >= drone.PassivesPerDrone)
                {
                    arenaShopHelperText.gameObject.SetActive(true);
                    arenaShopHelperText.text = "Unable to Add More Passive Modules to this Drone";
                    return null;
                }
                break;
            case ModuleCategory.WEAPON:
                if (drone.GetNumberOfModules(ModuleCategory.WEAPON) >= drone.WeaponsPerDrone)
                {
                    arenaShopHelperText.gameObject.SetActive(true);
                    arenaShopHelperText.text = "Unable to Add More Weapon Modules to this Drone";
                    return null;
                }
                break;
        }

        // Check Cost; if free, don't worry about cost/payment
        if (free)
        {
            ShopManager._Instance.UseFreePurchase();
        }
        else // Must have enough money, also payment
        {
            if (ShopManager._Instance.CurrentPlayerResource < cost)
            {
                arenaShopHelperText.gameObject.SetActive(true);
                arenaShopHelperText.text = "Insufficient Funds to Purchase Module";
                return null;
            }
            arenaShopHelperText.gameObject.SetActive(false);
            ShopManager._Instance.AlterResource(-cost);
        }

        // Actually add module
        return AddModule(drone, type);
    }

    public DroneModule AddModule(DroneController drone, ModuleType type)
    {
        return drone.AddModule(moduleTypeInfo.GetEntry(type).Value.Module);
    }

    public bool RemoveModule(DroneController drone, DroneModule module)
    {
        return drone.RemoveModule(module);
    }

    private void ResetScriptableObjects()
    {
        // Upgrade Nodes
        UpgradeNode[] upgradeNodes = Resources.LoadAll<UpgradeNode>("");
        foreach (UpgradeNode node in upgradeNodes)
        {
            // Debug.Log("Resetting: " + node);
            node.Reset();
        }

        // Stat Map
        perLevelEnemyStatMap.Reset();
    }

    public void IncreaseCameraZoom(float bossZoomOut)
    {
        // targetCameraZoom = targetCameraZoom + new Vector3(0, bossZoomOut, 0);
    }

    public void DecreaseCameraZoom(float bossZoomOut)
    {
        // targetCameraZoom = targetCameraZoom - new Vector3(0, bossZoomOut, 0);
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

    public void HardResetStatNodes()
    {
        UpgradeNode[] nodes = Resources.LoadAll<UpgradeNode>("UpgradeNodes");
        foreach (UpgradeNode node in nodes)
        {
            if (node is IUpgradeNodePermanantelyUpgradeable)
                ((IUpgradeNodePermanantelyUpgradeable)node).HardReset();
        }
    }
}
