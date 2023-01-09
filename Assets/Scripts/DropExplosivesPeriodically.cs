using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropExplosivesPeriodically : MonoBehaviour
{
    [SerializeField] private GameObject explosiveToDrop;
    [SerializeField] private StatModifier inBetweenDrops;

    private void OnEnable()
    {
        StartCoroutine(DropExplosive());
    }

    private void OnDisable()
    {
        StopCoroutine(DropExplosive());
    }

    private IEnumerator DropExplosive()
    {
        yield return new WaitForSeconds(inBetweenDrops.Value);

        Instantiate(explosiveToDrop, transform.position, Quaternion.identity);

        StartCoroutine(DropExplosive());
    }
}
