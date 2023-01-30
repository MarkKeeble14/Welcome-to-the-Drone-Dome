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
    [SerializeField] private bool awardInitialShopVisit;
    [SerializeField] private List<WaveWrapper> arenaWaves = new List<WaveWrapper>();
    private int currentWaveIndex;
    private List<GameObject> aliveCreepEnemies = new List<GameObject>();
    private List<GameObject> aliveBossEnemies = new List<GameObject>();
    private List<GameObject> otherSpawnedElements = new List<GameObject>();
    private bool clearedWave;
    private bool hasCompletedReward;
    public bool HasCompletedReward { get; private set; }
    private bool inArena;
    private bool isDone;
    private WaveWrapper currentWave;
    private Dictionary<GameObject, WaveWrapper> enemyWaveDictionary = new Dictionary<GameObject, WaveWrapper>();
    private bool CanClaimVictory => isDone && GameManager._Instance.OnLastLevel;
    private bool CreepDeathShouldCountTowardsProgress => !inPostWaveClearPeriod;
    private int numberEnemiesAllowedAlive;
    private int numberEnemiesToKill;

    private bool inPostWaveClearPeriod;
    private float postWaveClearPeriodDuration = 1f;

    [Header("References")]
    private BossInfoDisplay bossInfoDisplay;
    private EnemiesKilledBar progressBar;
    private ArenaInstructionsText arenaInstructionsText;
    private ArenaRewardText arenaRewardText;
    [SerializeField] private StoreInt playerCredits;
    private PlayerSpawning playerSpawning;
    [SerializeField] private PopupText popupText;
    private Transform player;


    [Header("Resource Clearing")]
    [SerializeField] private float timeBetweenResourceClears = 60f;
    [SerializeField] private float resourceClearFudgeFactor = 6f;
    private float ResourceClearFudgeFactor => timeBetweenResourceClears / resourceClearFudgeFactor;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource tickingSource;
    [SerializeField] private AudioClip bossWaveStartClip;
    [SerializeField] private AudioClip creepWaveStartClip;
    [SerializeField] private AudioClip waveCompletedClip;
    [SerializeField] private AudioClip arenaEndedClip;
    [SerializeField] private AudioClip arenaStartedClip;
    [SerializeField] private AudioClip inflationClip;

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
        playerSpawning = FindObjectOfType<PlayerSpawning>();

        arenaInstructionsText.SetText("Press R to Begin The Round!");
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
        if (inArena) return;
        if (CanClaimVictory)
            GameManager._Instance.Loop();
        else
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


    private void BeginArenaPressed(InputAction.CallbackContext ctx)
    {
        // Ensure that only one sequence may be ongoing at a time
        if (isDone) return;
        inArena = true;

        BeginArena();
    }

    public void BeginArena()
    {
        // Music
        AudioManager._Instance.StartLevelMusic();

        // Audio
        sfxSource.PlayOneShot(arenaStartedClip);

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

        StartCoroutine(ClearResourceSequence());

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
        if (isDone)
        {
            inArena = false;

            // Music
            AudioManager._Instance.StopLevelMusic();

            // Audio
            sfxSource.PlayOneShot(arenaEndedClip);

            // Player gains a credit every time they complete an arena
            int increase = Mathf.RoundToInt(GameManager._Instance.GetCreditBonus(progressBar.WavesCompleted));
            playerCredits.Value += increase;

            string plural = "";
            if (increase > 1)
            {
                plural += "s";
            }
            Instantiate(popupText, player.transform.position, Quaternion.identity)
                .Set("+" + increase + " Credit" + plural + "!\nNew Total: " + playerCredits.Value.ToString(), Color.yellow, player.transform.position, 3);

            StopClearResourceSequence();

            // Set texts to appropriate strings; important to not disable gameObjects here, as doing so will cause
            // the arena manager to not be able to find them when it does it's "FindObjectOfType" on start
            arenaInstructionsText.SetText(
                (!CanClaimVictory ? "Press Enter to Go To Next Level\nRemaining Loot Will be Autocollected" : "Press Enter to Loop") +
                (CanClaimVictory ? "\nPress F to Claim Victory" : "")
            );
            arenaRewardText.SetText("");
            yield break;
        }
        //  Debug.Log("Wave Loop Started: " + gameObject);

        // Set flags to false before starting next wave
        clearedWave = false;
        numberEnemiesAllowedAlive = (int)(currentWave.NumEnemiesAliveAtOnce * GameManager._Instance.EnemyStatMap.NumEnemiesAliveMod.Value);
        numberEnemiesToKill = (int)(currentWave.EnemiesToKill * GameManager._Instance.EnemyStatMap.NumEnemiesToKillMod.Value);

        if (currentWave.RewardType != WaveReward.NONE)
        {
            arenaRewardText.SetText(currentWave.RewardType.ToString());
        }
        else
        {
            arenaRewardText.SetText(currentWave.SubWaveRewards[0].Reward.ToString());
        }

        // Determine whether next wave contains a boss or not; start the appropriate wave sequence
        CallSequence(currentWave.HasBoss ? WaveType.BOSS : WaveType.CREEP);

        //  Debug.Log("Waiting To Clear Wave");

        yield return new WaitUntil(() => clearedWave);

        // Debug.Log("Waiting To Give Reward");

        yield return new WaitUntil(() => hasCompletedReward);

        // Debug.Log("Has Given Reward");

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
                StartCoroutine(BossSequence());
                break;
            case WaveType.CREEP:
                StartCoroutine(WaveSequence());
                break;
        }
    }

    private IEnumerator WaveSequence()
    {
        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(creepWaveStartClip);
        sfxSource.pitch = 1f;

        // Debug.Log("Wave Sequence Started");

        // Spawn Boss
        foreach (GameObject boss in currentWave.MiniBossSpawns)
        {
            // Instantiate both the boss and a corresponding health bar
            SpawnMiniBossEnemy(boss);
        }

        // Loop until wave has been cleared
        while (!clearedWave && !isDone)
        {
            if (!spawnEnemies) yield return null;
            if (aliveCreepEnemies.Count < numberEnemiesAllowedAlive)
            {
                SpawnNewCreepEnemy();
            }
            if (otherSpawnedElements.Count < currentWave.NumOtherSpawnsAtOnce)
            {
                SpawnOtherElement();
            }
            yield return null;
        }

        // Debug.Log("Wave Sequence Ended");
    }

    private IEnumerator BossSequence()
    {
        // Audio
        sfxSource.PlayOneShot(bossWaveStartClip);

        // Debug.Log("Boss Sequence Started");
        // Remove all creeps
        // ClearAllCreeps();

        // Spawn Boss
        foreach (GameObject boss in currentWave.BossSpawns)
        {
            // Instantiate both the boss and a corresponding health bar
            SpawnBossEnemy(boss);
        }

        // Loop until wave has been cleared
        while (!clearedWave && !isDone)
        {
            if (!spawnEnemies) yield return null;
            if (aliveCreepEnemies.Count < numberEnemiesAllowedAlive)
            {
                SpawnNewCreepEnemy();
            }
            if (otherSpawnedElements.Count < currentWave.NumOtherSpawnsAtOnce)
            {
                SpawnOtherElement();
            }
            yield return null;
        }

        // Debug.Log("Boss Sequence Ended");
    }

    public Vector3 GetRandomPosOnPlane(GameObject plane)
    {
        Vector3 newVec = new Vector3(plane.transform.position.x + Random.Range(-5f, 5f),
                                     player.position.y,
                                     plane.transform.position.z + Random.Range(-5f, 5f));
        return newVec;
    }

    private Vector3 GetSpawnPosition()
    {
        SpawningPad platform = playerSpawning.GetSpawnablePlatform();
        Vector3 spawnPos;
        if (platform == null)
        {
            spawnPos = Vector3.zero;
        }
        else
        {
            spawnPos = GetRandomPosOnPlane(platform.gameObject);
        }
        return spawnPos;
    }

    private void SpawnBossEnemy(GameObject bossVariant)
    {
        GameObject enemySpawned = Instantiate(bossVariant, GetSpawnPosition() + (Vector3.up * bossVariant.transform.localScale.y / 2), Quaternion.identity);
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
        };
    }

    private void SpawnMiniBossEnemy(GameObject bossVariant)
    {
        GameObject enemySpawned = Instantiate(bossVariant, GetSpawnPosition() + (Vector3.up * bossVariant.transform.localScale.y / 2), Quaternion.identity);
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
                UpdateProgressBar(spawnedDuringWave);
            }
        };
    }


    private void SpawnCreepEnemy(GameObject enemyVariant)
    {
        GameObject enemySpawned = Instantiate(enemyVariant, GetSpawnPosition() + (Vector3.up * enemyVariant.transform.localScale.y / 2), Quaternion.identity);
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
                UpdateProgressBar(spawnedDuringWave);
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

    private IEnumerator ClearResourceSequence()
    {
        yield return new WaitForSeconds(timeBetweenResourceClears + RandomHelper.RandomFloat(-ResourceClearFudgeFactor, ResourceClearFudgeFactor));

        // Collect all remaining resource
        AutoCollectScavengeable[] remainingAutoCollectable = FindObjectsOfType<AutoCollectScavengeable>();

        if (remainingAutoCollectable.Length > 0)
        {
            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.9f, 1.1f);
            sfxSource.PlayOneShot(inflationClip);
            sfxSource.pitch = 1;
            tickingSource.enabled = true;

            int numCollected = 0;
            foreach (AutoCollectScavengeable autoCollectable in remainingAutoCollectable)
            {
                autoCollectable.Expire(() => numCollected++);
            }

            yield return new WaitUntil(() => numCollected == remainingAutoCollectable.Length);
            tickingSource.enabled = false;
        }

        StartCoroutine(ClearResourceSequence());
    }

    private void StopClearResourceSequence()
    {
        StopCoroutine(ClearResourceSequence());

        AutoCollectScavengeable[] remainingAutoCollectable = FindObjectsOfType<AutoCollectScavengeable>();
        foreach (AutoCollectScavengeable autoCollectable in remainingAutoCollectable)
        {
            autoCollectable.CancelExpire();
        }
    }

    private IEnumerator PostWaveClearPeriod()
    {
        inPostWaveClearPeriod = true;

        yield return new WaitForSeconds(postWaveClearPeriodDuration);

        inPostWaveClearPeriod = false;
    }

    private void UpdateProgressBar(WaveWrapper wave)
    {
        float completionPercent = (float)wave.EnemiesKilledThisWave / numberEnemiesToKill;
        // If player has killed the neccessary number of enemies but there is still a boss alive, wait until the boss is dead
        if (completionPercent >= 1 && aliveBossEnemies.Count > 0)
        {
            progressBar.SetBar(1 - (aliveBossEnemies.Count * 0.01f));
        }
        else
        {
            progressBar.SetBar(completionPercent);
        }
    }

    private void ResetProgressBar()
    {
        progressBar.SetBar(0);
        progressBar.ResetHasCalled();
    }

    private void ClearedWave()
    {
        // Audio
        sfxSource.PlayOneShot(waveCompletedClip);

        progressBar.OnFill -= ClearedWave;
        progressBar.IncrementCounter();

        // Set cleared intervel to true so Wave Coroutine knows it has been completed
        clearedWave = true;

        // Start Grace Period
        StartCoroutine(PostWaveClearPeriod());

        Debug.Log("Cleared Wave");

        // Give the wave's reward
        GiveReward(currentWave);
        UpgradeManager._Instance.AddUpgradePoints(currentWave.UpgradePointsAwarded);

        // Increase tracker
        currentWaveIndex++;
        // Set new Current Wave
        if (currentWaveIndex < arenaWaves.Count)
        {
            currentWave = arenaWaves[currentWaveIndex];
            currentWave.EnemiesKilledThisWave = 0;
        }
        else
        {
            isDone = true;
        }
    }

    private void GiveReward(WaveWrapper wave)
    {
        foreach (SubWaveRewardWrapper subWaveReward in wave.SubWaveRewards)
        {
            switch (subWaveReward.Reward)
            {
                case SubWaveReward.NEW_DRONE:
                    AddDroneReward();
                    break;
                case SubWaveReward.MODULE_UNLOCKER:
                    ModuleUnlockerReward();
                    break;
                case SubWaveReward.MODULE_OVERCHARGER:
                    ModuleOverChargerReward();
                    break;
            }
        }

        hasCompletedReward = false;
        switch (wave.RewardType)
        {
            case WaveReward.UPGRADE:
                WeaponUpgradeReward();
                break;
            case WaveReward.SHOP_VISIT:
                ShopReward();
                break;
            case WaveReward.NONE:
                hasCompletedReward = true;
                break;
        }
    }

    private void ShopReward()
    {
        ShopManager._Instance.OpenShop();
    }

    private void ModuleUnlockerReward()
    {
        ShopManager._Instance.AddModuleUpgradeUnlocker();
        WeaponUpgradeReward();
    }

    private void ModuleOverChargerReward()
    {
        ShopManager._Instance.AddModuleUpgradeOverCharger();
        WeaponUpgradeReward();
    }

    private void WeaponUpgradeReward()
    {
        UpgradeManager._Instance.OpenUpgradeTree();
    }
    private void AddDroneReward()
    {
        GameManager._Instance.SpawnAndAddDroneToOrbit();
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
