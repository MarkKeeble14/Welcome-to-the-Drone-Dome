using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawning : MonoBehaviour
{
    [SerializeField] private List<SpawningPad> allPads = new List<SpawningPad>();
    [SerializeField] private List<SpawningPad> options = new List<SpawningPad>();

    private void Start()
    {
        SceneManager.activeSceneChanged += LoadPads;
    }

    private void LoadPads(Scene current, Scene next)
    {
        options.Clear();

        // Track all Pads
        allPads.Clear();
        allPads.AddRange(FindObjectsOfType<SpawningPad>());
    }

    public SpawningPad GetSpawnablePlatform()
    {
        if (options.Count == 0) return null;
        SpawningPad toReturn = options[RandomHelper.RandomIntExclusive(0, options.Count)];
        return toReturn;
    }

    private void SetOptions()
    {
        // Resetting Unimportant Pads
        foreach (SpawningPad pad in allPads)
        {
            if (pad.RealSpawnable && !options.Contains(pad))
            {
                options.Add(pad);
            }
            else if (!pad.RealSpawnable && options.Contains(pad))
            {
                options.Remove(pad);
            }
        }
    }

    private void Update()
    {
        SetOptions();
    }
}