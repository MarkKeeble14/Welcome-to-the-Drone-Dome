using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossInfoDisplay : MonoBehaviour
{
    [SerializeField] private BossBar bossBar;
    private Dictionary<GameObject, BossBar> spawnedDictionary = new Dictionary<GameObject, BossBar>();
    [SerializeField] private BossPointersDisplay bossPointersDisplay;

    public void SpawnNewDisplay(GameObject boss)
    {
        BossBar spawned = Instantiate(bossBar, transform);

        // Set Text and give reference to Boss
        spawned.Set(boss);
        boss.GetComponent<BossHealth>().HealthBar = spawned;

        bossPointersDisplay.Set(boss);

        spawnedDictionary.Add(boss, spawned);
    }

    public void RemoveDisplay(GameObject boss)
    {
        BossBar bar = spawnedDictionary[boss];

        bossPointersDisplay.Remove(boss);
        spawnedDictionary.Remove(boss);
        Destroy(bar.gameObject);
    }
}
