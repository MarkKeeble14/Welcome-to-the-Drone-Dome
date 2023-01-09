using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAndLoopStatMap", menuName = "LevelAndLoopStatMap", order = 1)]
public class LevelAndLoopStatMap : ScriptableObject
{
    [SerializeField] private EnemyStatMap[] enemyStatMaps;
    private int index;
    public EnemyStatMap Current => enemyStatMaps[index];

    public void OnLoop()
    {
        foreach (EnemyStatMap statMap in enemyStatMaps)
        {
            statMap.Grow();
        }
    }

    public void Reset()
    {
        foreach (EnemyStatMap enemyStatMap in enemyStatMaps)
        {
            enemyStatMap.Reset();
        }
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }
}
