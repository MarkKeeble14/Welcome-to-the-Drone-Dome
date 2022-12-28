using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }

    [Header("Game Related")]
    [SerializeField] private bool spawnEnemies = true;
    [SerializeField] private List<WaveWrapper> arenaWaves = new List<WaveWrapper>();
    private int currentWaveIndex;
    private List<GameObject> aliveCreepEnemies = new List<GameObject>();
    private List<GameObject> aliveBossEnemies = new List<GameObject>();
    private List<GameObject> otherSpawnedElements = new List<GameObject>();
    private int enemiesToKillTarget;
    private int enemiesKilledThisWave;
    private bool clearedWave;
    private bool isBossWave;
    private bool hasCompletedReward;
    public bool HasCompletedReward { get; private set; }

    [Header("References")]
    [SerializeField] private BossInfoDisplay bossInfoDisplay;
    [SerializeField] private Bar progressBar;
    [SerializeField] private PlayerDroneOrbitController playerDroneController;

    [Header("Settings")]
    [SerializeField] private float cameraWidthAllowance;
    [SerializeField] private float cameraHeightAllowance;
    private Transform player;

    private float enemiesAllowedAliveModifier = 1f;
    [SerializeField] private float enemiesAllowedAliveGrowth = 1.1f;

    public int GetNumberEnemiesAllowedAlive()
    {
        return (int)(GetCurrentWave().NumEnemiesAliveAtOnce * enemiesAllowedAliveModifier);
    }

    private void Start()
    {
        enemiesToKillTarget = GetCurrentWave().EnemiesToKill;

        // Get reference to player transform
        player = GameManager._Instance.Player;

        ShopReward();

        // Start Game
        StartCoroutine(Game());
    }

    private IEnumerator Game()
    {
        yield return new WaitUntil(() => hasCompletedReward);

        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        Debug.Log("Wave Loop Started");

        // Set flags to false before starting next wave
        clearedWave = false;

        // Determine whether next wave contains a boss or not; start the appropriate wave sequence
        if (GetCurrentWave().HasBoss)
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

        // Loop until wave has been cleared
        while (!clearedWave)
        {
            if (!spawnEnemies) yield return null;
            if (aliveCreepEnemies.Count < GetNumberEnemiesAllowedAlive())
            {
                SpawnNewCreepEnemy();
            }
            if (otherSpawnedElements.Count < GetCurrentWave().NumOtherSpawnsAtOnce)
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
        foreach (GameObject boss in GetCurrentWave().BossSpawns)
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
            if (otherSpawnedElements.Count < GetCurrentWave().NumOtherSpawnsAtOnce)
            {
                SpawnOtherElement();
            }
            yield return null;
        }

        Debug.Log("Boss Sequence Ended");
    }

    private Vector3 GetSpawnPosition()
    {
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
            Debug.Log("Removing from alive bosses");

            // Boss has been killed
            aliveBossEnemies.Remove(enemySpawned);

            // Update Tracking
            Debug.Log("Removing from display");
            bossInfoDisplay.RemoveDisplay(enemySpawned);

            Debug.Log("Updating Progress Bar");
            if (isBossWave)
            {
                progressBar.SetBar((float)aliveBossEnemies.Count / GetCurrentWave().BossSpawns.Count);
            }

            // Check if completed intervel
            if (aliveBossEnemies.Count <= 0)
            {
                ClearedWave();
            }
        };
    }

    private void SpawnCreepEnemy(GameObject enemyVariant)
    {
        GameObject enemySpawned = Instantiate(enemyVariant, GetSpawnPosition(), Quaternion.identity);
        aliveCreepEnemies.Add(enemySpawned);

        HealthBehaviour enemyHP = enemySpawned.GetComponent<HealthBehaviour>();
        enemyHP.OnDie += () =>
        {
            // Enemy has been killed
            aliveCreepEnemies.Remove(enemySpawned);

            // Update tracking
            enemiesKilledThisWave++;

            if (!isBossWave)
            {
                progressBar.SetBar((float)enemiesKilledThisWave / enemiesToKillTarget);

                // Check if completed intervel
                if (enemiesKilledThisWave >= enemiesToKillTarget)
                {
                    ClearedWave();
                }
            }
        };
    }

    private void SpawnNewCreepEnemy()
    {
        GameObject enemyToSpawn = GetCurrentWave().GetEnemySpawn();

        SpawnCreepEnemy(enemyToSpawn);
    }

    private void ClearAllCreeps()
    {
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
        GameObject otherToSpawn = GetCurrentWave().GetOtherSpawn();

        GameObject otherSpawned = Instantiate(otherToSpawn, GetSpawnPosition(), Quaternion.identity);
        otherSpawnedElements.Add(otherSpawned);

        Shoveable shoveable = otherSpawned.GetComponent<Shoveable>();
        shoveable.MyOnDestroy += () =>
        {
            otherSpawnedElements.Remove(otherSpawned);
        };
    }

    private int GetCurrentWaveIndex()
    {
        return currentWaveIndex % arenaWaves.Count;
    }

    private WaveWrapper GetCurrentWave()
    {
        return arenaWaves[GetCurrentWaveIndex()];
    }

    private void ResetProgressBar()
    {
        progressBar.SetBar(0);
    }

    private void ClearedWave()
    {
        Debug.Log("Cleared Wave");

        // Set cleared intervel to true so Wave Coroutine knows it has been completed
        clearedWave = true;

        // Give the wave's reward
        GiveAppropriateReward();
        UpgradeManager._Instance.AddUpgradePoints(GetCurrentWave().UpgradePointsAwarded);

        // Increase tracker
        currentWaveIndex++;

        // Set new goal
        enemiesKilledThisWave = 0;
        enemiesToKillTarget += GetCurrentWave().EnemiesToKill;
        ResetProgressBar();

        // Increase the max number of enemies alive at once
        enemiesAllowedAliveModifier *= enemiesAllowedAliveGrowth;
    }

    private void GiveAppropriateReward()
    {
        hasCompletedReward = false;
        switch (GetCurrentWave().RewardType)
        {
            case WaveReward.WEAPON_UPGRADE:
                // Debug.Log("Given Drone Upgrade");
                WeaponUpgradeReward();
                break;
            case WaveReward.NEW_ALPHA_DRONE:
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
