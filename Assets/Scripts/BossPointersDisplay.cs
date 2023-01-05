using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPointersDisplay : MonoBehaviour
{
    [SerializeField] private BossPointer bossPointerPrefab;
    private Dictionary<GameObject, BossPointer> spawnedDictionary = new Dictionary<GameObject, BossPointer>();
    [SerializeField] private Transform holder;
    public void Set(GameObject gameObject)
    {
        BossPointer spawned = Instantiate(bossPointerPrefab, holder);
        spawnedDictionary.Add(gameObject, spawned);
        spawned.Set(transform, gameObject);
    }

    public void Remove(GameObject gameObject)
    {
        BossPointer toDestroy = spawnedDictionary[gameObject];
        spawnedDictionary.Remove(gameObject);
        if (toDestroy != null)
            Destroy(toDestroy.gameObject);
    }
}
