using System;
using System.Collections;
using UnityEngine;

public abstract class AutoCollectScavengeable : Scavengeable
{
    [Header("Auto Collect")]
    [SerializeField] private float autoCollectSpeed = 5f;
    [SerializeField] private Vector2 chanceToAutoCollect = new Vector2(1, 4);
    [SerializeField] private float timeTakenToAutoCollectSpeedMultiplier = 2f;
    private float timeTakenToAutoCollect = 1;
    private bool setToAutoCollect;

    [Header("Expiry")]
    [SerializeField] private float expireSpeed = .1f;
    [SerializeField] private float growDuration = .25f;
    [SerializeField] private float growSpeed = .5f;
    [SerializeField] private float pauseDuration = 1f;
    private bool expiring;

    private void Update()
    {
        if (setToAutoCollect)
            timeTakenToAutoCollect += Time.deltaTime;
    }

    public void AutoCollect(Action action)
    {
        if (!setToAutoCollect)
            StartCoroutine(ExecuteAutoCollect(action));
    }

    public void Expire()
    {
        if (!expiring)
            StartCoroutine(ExpirationSequence());
    }

    private IEnumerator ExpirationSequence()
    {
        expiring = true;

        // Grow a little
        for (float t = 0; t < growDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale + Vector3.one, growSpeed * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(pauseDuration);

        // Shrink to 0 then destroy
        while (transform.localScale != Vector3.zero)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * expireSpeed);

            yield return null;
        }

        Destroy(transform.root.gameObject);
    }

    private IEnumerator ExecuteAutoCollect(Action action)
    {
        setToAutoCollect = true;

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
            Destroy(transform.root.gameObject);
        }
    }
}
