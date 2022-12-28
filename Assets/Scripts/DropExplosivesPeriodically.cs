using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropExplosivesPeriodically : MonoBehaviour
{
    [SerializeField] private GameObject explosiveToDrop;
    [SerializeField] private float inBetweenDrops;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DropExplosive());
    }

    private IEnumerator DropExplosive()
    {
        yield return new WaitForSeconds(inBetweenDrops);

        Instantiate(explosiveToDrop, transform.position, Quaternion.identity);

        StartCoroutine(DropExplosive()); ;
    }
}
