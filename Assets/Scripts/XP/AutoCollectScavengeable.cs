using System;
using System.Collections;
using UnityEngine;

public abstract class AutoCollectScavengeable : Scavengeable
{
    [SerializeField] private float autoCollectSpeed = 5f;
    [SerializeField] private Vector2 chanceToAutoCollect = new Vector2(1, 4);
    [SerializeField] private float timeTakenToAutoCollectSpeedMultiplier = 2f;
    private float timeTakenToAutoCollect = 1;
    private bool setToAutoCollect;

    private void Update()
    {
        if (setToAutoCollect)
            timeTakenToAutoCollect += Time.deltaTime;
    }

    public void AutoCollect(Action action)
    {
        setToAutoCollect = true;
        StartCoroutine(ExecuteAutoCollect(action));
    }

    private IEnumerator ExecuteAutoCollect(Action action)
    {
        // Disable collider
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;

        // Cache the player to reduce number of accesses
        Transform cachedPlayer = GameManager._Instance.Player;

        // Move towards player position
        while (Vector3.Distance(transform.position, cachedPlayer.position) > .5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, cachedPlayer.position,
                Time.deltaTime * autoCollectSpeed * (timeTakenToAutoCollect * timeTakenToAutoCollectSpeedMultiplier));

            yield return null;
        }

        action();

        // Determine if should be added to player reserves or not
        if (RandomHelper.RandomIntExclusive(chanceToAutoCollect) <= chanceToAutoCollect.x)
        {
            OnPickup();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
