using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveWrapper
{
    [Header("Rewards")]
    public WaveReward RewardType;
    public List<SubWaveRewardWrapper> SubWaveRewards = new List<SubWaveRewardWrapper>();
    public int UpgradePointsAwarded;
    public bool CanRepeatedlyAwardUpgradePoints = true;
    public bool CanRepeatWaveReward = true;

    [Header("Spawns")]
    public PercentageMap<GameObject> EnemySpawns = new PercentageMap<GameObject>();
    public PercentageMap<GameObject> OtherSpawns = new PercentageMap<GameObject>();

    [Header("Enemies")]
    [HideInInspector] public int EnemiesKilledThisWave;
    public int EnemiesToKill;
    public int NumEnemiesAliveAtOnce;
    public int NumOtherSpawnsAtOnce;

    public bool HasBoss => BossSpawns.Count > 0;
    public float BossZoomOut;

    public List<GameObject> BossSpawns = new List<GameObject>();
    public List<GameObject> MiniBossSpawns = new List<GameObject>();

    public GameObject GetEnemySpawn()
    {
        return EnemySpawns.GetOption();
    }

    public GameObject GetOtherSpawn()
    {
        return OtherSpawns.GetOption();
    }
}
