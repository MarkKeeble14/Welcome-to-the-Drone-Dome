using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperMortarProjectile : MortarProjectile
{
    [SerializeField] private StatModifier numThumps;
    [SerializeField] private StatModifier timeBetweenThumps;
    [SerializeField] private GameObject dissapearParticle;
    private bool activated;

    private void OnEnable()
    {
        activated = false;
    }

    public override void ArrivedAtPosition()
    {
        if (activated) return;
        activated = true;
        StartCoroutine(StartThump());
    }

    private IEnumerator StartThump()
    {
        List<ThumpRing> spawnedRings = new List<ThumpRing>();

        for (int i = 0; i < numThumps.Value; i++)
        {
            ThumpRing spawned = ObjectPooler.thumpRingPool.Get();
            spawnedRings.Add(spawned);
            spawned.transform.position = transform.position;

            StartCoroutine(spawned.ExecuteThump(() =>
            {
                spawnedRings.Remove(spawned);
                ObjectPooler.thumpRingPool.Release(spawned);
                // Debug.Log("Removed: " + spawned + ", Remaining: " + spawnedRings.Count);
            }));

            if (i != numThumps.Value - 1)
                yield return new WaitForSeconds(timeBetweenThumps.Value);
        }

        Instantiate(dissapearParticle, transform.position, Quaternion.identity);

        yield return new WaitUntil(() => spawnedRings.Count == 0);

        ReleaseAction?.Invoke();
    }
}
