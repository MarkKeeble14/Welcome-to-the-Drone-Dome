using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class ArenaManager : MonoBehaviour
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
    private EnemiesKilledBar progressBar;
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

    private bool inPostWaveClearPeriod;
    private float postWaveClearPeriodDuration = 1f;

    private bool ShouldGiveReward => onLoop == 0 || (onLoop > 0 && currentWave.CanRepeatWaveReward);
    private bool ShouldAwardUpgradePoints => onLoop == 0 || (onLoop > 0 && currentWave.CanRepeatedlyAwardUpgradePoints);
    private bool CanClaimVictory => currentWaveIndex >= arenaWaves.Count - 1
        && GameManager._Instance.OnLastLevel && !hasBegun;
    private bool CreepDeathShouldCountTowardsProgress => !inPostWaveClearPeriod && !isBossWave;

    private void Start()
    {
        // Add Controls
        InputManager._Controls.Player.BeginArena.started += BeginArenaPressed;
        InputManager._Controls.Player.NextLevel.started += NextLevelPressed;
        InputManager._Controls.Player.Win.started += WinPressed;

        // Get references
        arenaInstructionsText = FindObjectOfType<ArenaInstructionsText>();
        arenaRewardText = FindObjectOfType<ArenaRewardText>();
        progressBar = FindObjectOfType<EnemiesKilledBar>();
        bossInfoDisplay = FindObjectOfType<BossInfoDisplay>();

        arenaInstructionsText.SetText("Press R to Begin The Challenge!");
    }

    private void NextLevelPressed(InputAction.CallbackContext ctx)
    {
        InputManager._Controls.Player.BeginArena.started -= BeginArenaPressed;
        InputManager._Controls.Player.NextLevel.started -= NextLevelPressed;
        InputManager._Controls.Player.Win.started -= WinPressed;

        TryNextLevel();
    }


    private void TryNextLevel()
    {
        // Debug.Log(onLoop + ", " + hasBegun);
        if (onLoop == -1) return;
        if (hasBegun) return;
        GameManager._Instance.LoadNextLevel();
    }

    private void WinPressed(InputAction.CallbackContext ctx)
    {
        if (!CanClaimVictory) return;
        ClearAllCreeps();
        StopAllCoroutines();
        GameManager._Instance.Win();
        return;
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
            arenaInstructionsText.SetText(
                (!CanClaimVictory ? "Press Enter to Go To Next Level" : "") +
                (allowLooping ? "\nPress R to Challenge Again!" : "") +
                (CanClaimVictory ? "\nPress F to Claim Vicory" : "")
            );
            arenaRewardText.SetText("");
            yield break;
        }
        Debug.Log("Wave Loop Started: " + gameObject);

        // Set flags to false before starting next wave
        clearedWave = false;
        // Set Reward Text
        if (ShouldGiveReward)
        {
            arenaRewardText.SetText(currentWave.RewardType.ToString());
        }
        else
        {
            arenaRewardText.SetText(WaveReward.NONE.ToString());
        }

        // Determine whether next wave contains a boss or not; start the appropriate wave sequence
        CallSequence(currentWave.HasBoss ? WaveType.BOSS : WaveType.CREEP);

        Debug.Log("Waiting To Clear Wave");

        yield return new WaitUntil(() => clearedWave);

        Debug.Log("Waiting To Give Reward");

        yield return new WaitUntil(() => hasCompletedReward);

        Debug.Log("Has Given Reward");

        StartCoroutine(WaveLoop());
    }

    private void CallSequence(WaveType type)
    {
        // Reset the progress bar
        ResetProgressBar();
        progressBar.OnFill += ClearedWave;

        switch (type)
        {
            case WaveType.BOSS:
                isBossWave = true;
                StartCoroutine(BossSequence());
                break;
            case WaveType.CREEP:
                isBossWave = false;
                StartCoroutine(WaveSequence());
                break;
        }
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
                progressBar.SetBar((currentWave.BossSpawns.Count - aliveBossEnemies.Count)
                    / (float)currentWave.BossSpawns.Count);
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
            // Update tracking
            WaveWrapper spawnedDuringWave = currentWave;
            enemyWaveDictionary.Remove(enemySpawned);
            spawnedDuringWave.EnemiesKilledThisWave++;

            if (CreepDeathShouldCountTowardsProgress)
            {
                float completionPercent = (float)spawnedDuringWave.EnemiesKilledThisWave / spawnedDuringWave.EnemiesToKill;
                progressBar.SetBar(completionPercent);
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
            WaveWrapper spawnedDuringWave = currentWave;
            enemyWaveDictionary.Remove(enemySpawned);
            spawnedDuringWave.EnemiesKilledThisWave++;

            if (CreepDeathShouldCountTowardsProgress)
            {
                float completionPercent = (float)spawnedDuringWave.EnemiesKilledThisWave / spawnedDuringWave.EnemiesToKill;
                progressBar.SetBar(completionPercent);
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

    private IEnumerator PostWaveClearPeriod()
    {
        inPostWaveClearPeriod = true;

        yield return new WaitForSeconds(postWaveClearPeriodDuration);

        inPostWaveClearPeriod = false;
    }

    private void ResetProgressBar()
    {
        progressBar.SetBar(0);
        progressBar.ResetHasCalled();
    }

    private void ClearedWave()
    {
        progressBar.OnFill -= ClearedWave;

        // Set cleared intervel to true so Wave Coroutine knows it has been completed
        clearedWave = true;

        // Start Grace Period
        StartCoroutine(PostWaveClearPeriod());

        Debug.Log("Cleared Wave");

        // Player has beat the last wave in the last level, so they would have won
        // However I think I want to allow them to loop to their hearts content, and
        /*
        if (currentWaveIndex == arenaWaves.Count - 1 && GameManager._Instance.OnLastLevel)
        {
            ClearAllCreeps();
            StopAllCoroutines();
            GameManager._Instance.Win();
            return;
        }
        */

        // Give the wave's reward
        if (ShouldGiveReward)
        {
            GiveReward(currentWave.RewardType);
        }
        else
        {
            GiveReward(WaveReward.NONE);
        }
        // Award upgrade points
        if (ShouldAwardUpgradePoints)
        {
            UpgradeManager._Instance.AddUpgradePoints(currentWave.UpgradePointsAwarded);
        }

        // Increase the max number of enemies alive at once
        enemiesAllowedAliveModifier *= enemiesAllowedAliveGrowth;

        // Increase tracker
        currentWaveIndex++;
        // Set new Current Wave
        if (currentWaveIndex < arenaWaves.Count)
        {
            currentWave = arenaWaves[currentWaveIndex];
            currentWave.EnemiesKilledThisWave = 0;
        }
    }

    private void GiveReward(WaveReward reward)
    {
        hasCompletedReward = false;
        switch (reward)
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
