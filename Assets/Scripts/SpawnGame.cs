using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGame : MonoBehaviour
{
    public static SpawnGame _Instance { get; private set; }
    private void Awake()
    {
        // Set this as the new instance if there is an old one that exists
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;

        // Create New Instance of Game Elements
        Instantiate(gamePrefab);
    }

    [SerializeField] private GameElements gamePrefab;
}
