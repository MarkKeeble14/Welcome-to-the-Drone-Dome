using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperMortarProjectile : MortarProjectile
{
    [Header("Upgradeables")]
    [SerializeField] private int numThumps;
    [SerializeField] private float timeBetweenThumps;
    [SerializeField] private float radius;
    [SerializeField] private float expansionSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float knockback;

    public void SetStats(int numThumps, float timeBetweenThumps, float radius, float expansionSpeed, float damage, float knockback)
    {
        this.numThumps = numThumps;
        this.timeBetweenThumps = timeBetweenThumps;
        this.radius = radius;
        this.expansionSpeed = expansionSpeed;
        this.damage = damage;
        this.knockback = knockback;
    }

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

        for (int i = 0; i < numThumps; i++)
        {
            ThumpRing spawned = ObjectPooler.thumpRingPool.Get();
            spawnedRings.Add(spawned);
            spawned.transform.position = transform.position;

            StartCoroutine(spawned.ExecuteThump(() =>
            {
                spawnedRings.Remove(spawned);
                ObjectPooler.thumpRingPool.Release(spawned);
                // Debug.Log("Removed: " + spawned + ", Remaining: " + spawnedRings.Count);
            }, radius, expansionSpeed, damage, knockback));

            if (i != numThumps - 1)
                yield return new WaitForSeconds(timeBetweenThumps);
        }

        Instantiate(dissapearParticle, transform.position, Quaternion.identity);

        yield return new WaitUntil(() => spawnedRings.Count == 0);

        ReleaseAction?.Invoke();
    }
}
