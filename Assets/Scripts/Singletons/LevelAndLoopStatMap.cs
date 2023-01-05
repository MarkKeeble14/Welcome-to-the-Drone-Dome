using UnityEngine;

[CreateAssetMenu(fileName = "LevelAndLoopStatMap", menuName = "LevelAndLoopStatMap", order = 1)]
public class LevelAndLoopStatMap : ScriptableObject
{
    [SerializeField] private EnemyStatMap[] enemyStatMaps;
    private int index;
    public EnemyStatMap Current => enemyStatMaps[index];

    public void Next()
    {
        if (index + 1 < enemyStatMaps.Length)
        {
            index++;
        }
    }

    public void OnLoop()
    {
        Current.Grow();
    }

    public void Reset()
    {
        foreach (EnemyStatMap enemyStatMap in enemyStatMaps)
        {
            enemyStatMap.Reset();
        }
    }
}
