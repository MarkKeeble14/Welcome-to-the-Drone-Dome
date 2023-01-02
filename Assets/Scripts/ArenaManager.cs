using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [Header("Game Related")]
    [SerializeField] private bool spawnEnemies = true;
    [SerializeField] private bool allowLooping;
    [SerializeField] private bool awardInitialShopVisit;
    [SerializeField] private List<WaveWrapper> arenaWaves = new List<WaveWrapper>();
    private int currentWaveIndex;
    private List<GameObject> aliveCreepEnemies = new List<GameObject>();
    private List<GameObject> aliveBossEnemies = new List<GameObject>();
    private List<GameObject> otherSpawnedElements = new List<GameObject>();
    private bool clearedWave;
    private bool isBossWave;
    private bool hasCompletedReward;
    public bool HasCompletedReward { get; private set; }

    [Header("References")]
    private BossInfoDisplay bossInfoDisplay;
    private ProgressBar progressBar;
    private ArenaInstructionsText arenaInstructionsText;
    private ArenaRewardText arenaRewardText;

    [Header("Settings")]
    [SerializeField] private float cameraWidthAllowance = 20;
    [SerializeField] private float cameraHeightAllowance = 10;
    private Transform player;

    private float enemiesAllowedAliveModifier = 1f;
    [SerializeField] private float enemiesAllowedAliveGrowth = 1.1f;
    private bool hasBegun;
    private int onLoop = -1;
    private WaveWrapper currentWave;
    private Dictionary<GameObject, WaveWrapper> enemyWaveDictionary = new Dictionary<GameObject, WaveWrapper>();
    private CinemachineVirtualCamera vcam;

    private float durationOfPostWaveClearPeriod = .25f;
    private bool inPostWaveClearPeriod;

    private void Start()
    {
        // Add Controls
        InputManager._Controls.Player.BeginArena.started += BeginArenaPressed;
        InputManager._Controls.Player.NextLevel.started += NextLevelPressed;

        // Get references
        arenaInstructionsText = FindObjectOfType<ArenaInstructionsText>();
        arenaRewardText = FindObjectOfType<ArenaRewardText>();
        progressBar = FindObjectOfType<ProgressBar>();
        bossInfoDisplay = FindObjectOfType<BossInfoDisplay>();
        vcam = FindObjectOfType<CinemachineVirtualCamera>();

        arenaInstructionsText.SetText("Press R to Begin The Challenge!");
    }

    private void NextLevelPressed(InputAction.CallbackContext ctx)
    {
        InputManager._Controls.Player.BeginArena.started -= BeginArenaPressed;
        InputManager._Controls.Player.NextLevel.started -= NextLevelPressed;

        TryNextLevel();
    }

    private void TryNextLevel()
    {
        // Debug.Log(onLoop + ", " + hasBegun);
        if (onLoop == -1) return;
        if (hasBegun) return;
        GameManager._Instance.LoadNextLevel();
    }

    public int GetNumberEnemiesAllowedAlive()
    {
        return (int)(currentWave.NumEnemiesAliveAtOnce * enemiesAllowedAliveModifier);
    }

    private void BeginArenaPressed(InputAction.CallbackContext ctx)
    {
        // If we don't allow looping, don't allow the player to loop basically
        if (onLoop > -1 && !allowLooping) return;

        // Ensure that only one sequence may be ongoing at a time
        if (hasBegun) return;
        hasBegun = true;

        // Increment loop count
        onLoop++;

        BeginArena();
    }

    public void BeginArena()
    {
        // Turn off text
        arenaInstructionsText.Hide();

        // Reset currentWaveIndex
        currentWaveIndex = 0;
        // Set Current Wave
        currentWave = arenaWaves[currentWaveIndex];

        // Get reference to player transform
        player = GameManager._Instance.Player;

        // Give a shop reward
        if (awardInitialShopVisit)
            ShopReward();

        // Start Game
        StartCoroutine(ArenaSequence());
    }

    private IEnumerator ArenaSequence()
    {
        if (awardInitialShopVisit)
            yield return new WaitUntil(() => hasCompletedReward);

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        // If not allowing looping, then we stop once we've cleared all waves
        if (currentWaveIndex > arenaWaves.Count - 1)
        {
            ClearAllCreeps();
            hasBegun = false;

            // Set texts to appropriate strings; important to not disable gameObjects here, as doing so will cause
            // the arena manager to not be able to find them when it does it's "FindObjectOfType" on start
            arenaInstructionsText.SetText("Press Enter to Go To Next Level" + (allowLooping ? "\nPress R to Challenge Again!" : ""));
            arenaRewardText.SetText("");
            yield break;
        }
        Debug.Log("Wave Loop Started: " + gameObject);

        // Set flags to false before starting next wave
        clearedWave = false;
        // Set Reward Text
        arenaRewardText.SetText(currentWave.RewardType.ToString());
        // Reset the progress bar
        ResetProgressBar();

        // Determine whether next wave contains a boss or not; start the appropriate wave sequence
        if (currentWave.HasBoss)
        {
            isBossWave = true;
            StartCoroutine(BossSequence());
        }
        else
        {
            isBossWave = false;
            StartCoroutine(WaveSequence());
        }

        Debug.Log("Waiting To Clear Wave");

        yield return new WaitUntil(() => clearedWave);

        Debug.Log("Waiting To Give Reward");

        yield return new WaitUntil(() => hasCompletedReward);

        Debug.Log("Has Given Reward");

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveSequence()
    {
        Debug.Log("Wave Sequence Started");

        // Spawn Boss
        foreach (GameObject boss in currentWave.MiniBossSpawns)
        {
            // Instantiate both the boss and a corresponding health bar
            SpawnMiniBossEnemy(boss);
        }

        // Loop until wave has been cleared
        while (!clearedWave)
        {
            if (!spawnEnemies) yield return null;
            if (aliveCreepEnemies.Count < GetNumberEnemiesAllowedAlive())
            {
                SpawnNewCreepEnemy();
            }
            if (otherSpawnedElements.Count < currentWave.NumOtherSpawnsAtOnce)
            {
                SpawnOtherElement();
            }
            yield return null;
        }

        Debug.Log("Wave Sequence Ended");
    }

    private IEnumerator BossSequence()
    {
        Debug.Log("Boss Sequence Started");
        // Remove all creeps
        // ClearAllCreeps();

        // Spawn Boss
        foreach (GameObject boss in currentWave.BossSpawns)
        {
            // Instantiate both the boss and a corresponding health bar
            SpawnBossEnemy(boss);
        }

        // Zoom out
        GameManager._Instance.IncreaseCameraZoom(currentWave.BossZoomOut);

        // Loop until wave has been cleared
        while (!clearedWave)
        {
            if (!spawnEnemies) yield return null;
            if (aliveCreepEnemies.Count < GetNumberEnemiesAllowedAlive())
            {
                SpawnNewCreepEnemy();
            }
            if (otherSpawnedElements.Count < currentWave.NumOtherSpawnsAtOnce)
            {
                SpawnOtherElement();
            }
            yield return null;
        }

        // Zoom back in
        GameManager._Instance.DecreaseCameraZoom(currentWave.BossZoomOut);

        Debug.Log("Boss Sequence Ended");
    }

    private Vector3 GetSpawnPosition()
    {
        if (player == null) return Vector3.zero;
        int spawnSide = RandomHelper.RandomIntExclusive(0, 4);
        Vector3 r = player.position;
        switch (spawnSide)
        {
            case 0:// Left
                r += new Vector3(-cameraWidthAllowance, 0,
                    RandomHelper.RandomFloat(-cameraHeightAllowance / 2, cameraHeightAllowance / 2));
                break;
            case 1:// Right
                r += new Vector3(cameraWidthAllowance, 0,
                    RandomHelper.RandomFloat(-cameraHeightAllowance / 2, cameraHeightAllowance / 2));
                break;
            case 2:// Up
                r += new Vector3(RandomHelper.RandomFloat(-cameraWidthAllowance / 2, cameraWidthAllowance / 2), 0,
                    cameraHeightAllowance);
                break;
            case 3://Down
                r += new Vector3(RandomHelper.RandomFloat(-cameraWidthAllowance / 2, cameraWidthAllowance / 2), 0,
                    -cameraHeightAllowance);
                break;
        }
        return r;
    }

    private void SpawnBossEnemy(GameObject bossVariant)
    {
        GameObject enemySpawned = Instantiate(bossVariant, GetSpawnPosition(), Quaternion.identity);
        aliveBossEnemies.Add(enemySpawned);

        bossInfoDisplay.SpawnNewDisplay(enemySpawned);

        HealthBehaviour enemyHP = enemySpawned.GetComponent<HealthBehaviour>();
        enemyHP.OnDie += () =>
        {
            // Debug.Log("Removing from alive bosses");

            // Boss has been killed
            aliveBossEnemies.Remove(enemySpawned);

            // Update Tracking
            // Debug.Log("Removing boss info from display");
            bossInfoDisplay.RemoveDisplay(enemySpawned);

            // Debug.Log("Updating Progress Bar");
            if (isBossWave)
            {
                progressBar.SetBar((float)aliveBossEnemies.Count / currentWave.BossSpawns.Count);
            }

            // Check if completed intervel
            if (aliveBossEnemies.Count <= 0)
            {
                ClearedWave();
            }
        };
    }

    private void SpawnMiniBossEnemy(GameObject bossVariant)
    {
        GameObject enemySpawned = Instantiate(bossVariant, GetSpawnPosition(), Quaternion.identity);
        aliveCreepEnemies.Add(enemySpawned);
        enemyWaveDictionary.Add(enemySpawned, currentWave);

        // Add to Boss Info Display
        bossInfoDisplay.SpawnNewDisplay(enemySpawned);

        HealthBehaviour enemyHP = enemySpawned.GetComponent<HealthBehaviour>();
        enemyHP.OnDie += () =>
        {
            // Debug.Log("Removing from alive bosses");
            // Enemy has been killed
            aliveCreepEnemies.Remove(enemySpawned);

            // Debug.Log("Removing from display");
            bossInfoDisplay.RemoveDisplay(enemySpawned);

            // Grace period upon clearing a wave to prevent from overkilling and clearing multiple waves at once
            if (inPostWaveClearPeriod) return;
            // Update tracking
            WaveWrapper spawnedDuringWave = currentWave;
            enemyWaveDictionary.Remove(enemySpawned);
            spawnedDuringWave.EnemiesKilledThisWave++;

            if (!isBossWave)
            {
                float completionPercent = (float)spawnedDuringWave.EnemiesKilledThisWave / spawnedDuringWave.EnemiesToKill;
                progressBar.SetBar(completionPercent);

                // Check if completed intervel
                if (completionPercent >= 1)
                {
                    ClearedWave();
                }
            }
        };
    }

    private void SpawnCreepEnemy(GameObject enemyVariant)
    {
        GameObject enemySpawned = Instantiate(enemyVariant, GetSpawnPosition(), Quaternion.identity);
        aliveCreepEnemies.Add(enemySpawned);
        enemyWaveDictionary.Add(enemySpawned, currentWave);

        HealthBehaviour enemyHP = enemySpawned.GetComponent<HealthBehaviour>();
        enemyHP.OnDie += () =>
        {
            // Enemy has been killed
            aliveCreepEnemies.Remove(enemySpawned);

            // Update tracking
            // Grace period upon clearing a wave to prevent from overkilling and clearing multiple waves at once
            if (inPostWaveClearPeriod) return;
            WaveWrapper spawnedDuringWave = currentWave;
            enemyWaveDictionary.Remove(enemySpawned);
            spawnedDuringWave.EnemiesKilledThisWave++;

            if (!isBossWave)
            {
                float completionPercent = (float)spawnedDuringWave.EnemiesKilledThisWave / spawnedDuringWave.EnemiesToKill;
                progressBar.SetBar(completionPercent);

                // Check if completed intervel
                if (completionPercent >= 1)
                {
                    ClearedWave();
                }
            }
        };
    }

    private void SpawnNewCreepEnemy()
    {
        GameObject enemyToSpawn = currentWave.GetEnemySpawn();

        SpawnCreepEnemy(enemyToSpawn);
    }

    private void ClearAllCreeps()
    {
        // Clearing through overlord programming powers

        // In the future, might be cool to have them all scurry away instead of just destroying them
        while (aliveCreepEnemies.Count > 0)
        {
            GameObject creep = aliveCreepEnemies[0];
            aliveCreepEnemies.Remove(creep);
            Destroy(creep);
        }

        // Clearing through damage; will currently cause a bug wherein levels are completed
        /*
        foreach (GameObject creep in aliveCreepEnemies)
        {
            HealthBehaviour health = creep.GetComponent<HealthBehaviour>();
            health.Damage(health.MaxHealth);
        }
        */
    }

    private void SpawnOtherElement()
    {
        GameObject otherToSpawn = currentWave.GetOtherSpawn();

        GameObject otherSpawned = Instantiate(otherToSpawn, GetSpawnPosition(), Quaternion.identity);
        otherSpawnedElements.Add(otherSpawned);

        Shoveable shoveable = otherSpawned.GetComponent<Shoveable>();
        shoveable.CallOnDestroy += () =>
        {
            otherSpawnedElements.Remove(otherSpawned);
        };
    }

    private void ResetProgressBar()
    {
        progressBar.SetBar(0);
    }

    private void ClearedWave()
    {
        Debug.Log("Cleared Wave");

        // Player has beat the last wave in the last level, so they have won
        if (currentWaveIndex == arenaWaves.Count - 1 && GameManager._Instance.OnLastLevel)
        {
            ClearAllCreeps();
            StopAllCoroutines();
            GameManager._Instance.Win();
            return;
        }
        StartCoroutine(PostWaveClearPeriod());

        // Set cleared intervel to true so Wave Coroutine knows it has been completed
        clearedWave = true;

        // Give the wave's reward
        UpgradeManager._Instance.AddUpgradePoints(currentWave.UpgradePointsAwarded);
        GiveAppropriateReward(currentWave);

        // Increase the max number of enemies alive at once
        enemiesAllowedAliveModifier *= enemiesAllowedAliveGrowth;

        // Increase tracker
        currentWaveIndex++;
        // Set new Current Wave
        if (currentWaveIndex < arenaWaves.Count)
            currentWave = arenaWaves[currentWaveIndex];
    }

    private IEnumerator PostWaveClearPeriod()
    {
        inPostWaveClearPeriod = true;

        yield return new WaitForSeconds(durationOfPostWaveClearPeriod);

        inPostWaveClearPeriod = false;
    }

    private void GiveAppropriateReward(WaveWrapper wave)
    {
        hasCompletedReward = false;
        switch (wave.RewardType)
        {
            case WaveReward.UPGRADE:
                // Debug.Log("Given Drone Upgrade");
                WeaponUpgradeReward();
                break;
            case WaveReward.NEW_DRONE:
                // Debug.Log("Given New Alpha Drone");
                AddDroneReward();
                hasCompletedReward = true;
                break;
            case WaveReward.SHOP_VISIT:
                // Debug.Log("Given Shop Visit");
                ShopReward();
                break;
            case WaveReward.NONE:
                // Debug.Log("Given No Reward");
                hasCompletedReward = true;
                break;
        }
    }

    private void ShopReward()
    {
        ShopManager._Instance.OpenShop();
    }

    private void AddDroneReward()
    {
        GameManager._Instance.SpawnAndAddDroneToOrbit();
    }

    private void WeaponUpgradeReward()
    {
        UpgradeManager._Instance.OpenUpgradeTree();
    }

    public void ConfirmDoneShopVisit()
    {
        ShopManager._Instance.CloseShop();
        hasCompletedReward = true;
    }

    public void ConfirmDoneUpgrading()
    {
        UpgradeManager._Instance.CloseUpgradeTree();
        hasCompletedReward = true;
    }
}
