using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicObjectSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask cantSpawnOnTopOf;
    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private Vector3 positionCheck;
    [SerializeField] private float positionCheckRadius;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private GameObject toSpawn;

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(timeBetweenSpawn);

        Collider[] overlap = Physics.OverlapSphere(positionCheck, positionCheckRadius, cantSpawnOnTopOf);
        if (overlap.Length <= 0)
        {
            Instantiate(toSpawn, spawnPosition, Quaternion.identity);
        }
        StartCoroutine(SpawnLoop());
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }
}
