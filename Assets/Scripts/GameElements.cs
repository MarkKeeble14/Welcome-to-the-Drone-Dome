using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameElements : MonoBehaviour
{
    public static GameElements _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            _Instance.DestroyGameElements();
        }
        _Instance = this;
    }

    public List<GameObject> AddedObjects = new List<GameObject>();
    public void DestroyGameElements()
    {
        while (AddedObjects.Count > 0)
        {
            GameObject toDestroy = AddedObjects[0];
            // Debug.Log(toDestroy);
            AddedObjects.Remove(toDestroy);
            Destroy(toDestroy);
        }
        // Debug.Log(gameObject);
        Destroy(gameObject);
    }
}
