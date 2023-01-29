using System;
using System.Collections;
using UnityEngine;

public abstract class AutoCollectScavengeable : Scavengeable
{
    [Header("Auto Collect")]
    [SerializeField] private float autoCollectSpeed = 5f;
    [SerializeField] private Vector2 chanceToAutoCollect = new Vector2(1, 4);
    [SerializeField] private float timeTakenToAutoCollectSpeedMultiplier = 2f;
    private bool setToAutoCollect;

    [Header("Expiry")]
    [SerializeField] private float expireSpeed = .1f;
    [SerializeField] private float growDuration = .25f;
    [SerializeField] private float growSpeed = .5f;
    [SerializeField] private float pauseDuration = 1f;
    private bool expiring;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] col;

    public void AutoCollect(Action action)
    {
        if (!setToAutoCollect)
            StartCoroutine(AutoCollectSequence(action));
    }

    public void FailAutoCollect()
    {
        StopAllCoroutines();
        UndoDisableWhileCollecting();
        ReleaseToPool();
    }

    public void Expire()
    {
        if (!expiring)
            StartCoroutine(ExpirationSequence());
    }

    public void CancelExpire()
    {
        StopCoroutine(ExpirationSequence());
        expiring = false;
        StartCoroutine(ResetSize());
    }

    private IEnumerator ResetSize()
    {
        while (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * growSpeed);

            yield return null;
        }
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

        ReleaseToPool();
    }

    private void ToDisableWhileCollecting()
    {
        foreach (Collider collider in col)
        {
            collider.enabled = false;
        }
        rb.useGravity = false;
    }

    private void UndoDisableWhileCollecting()
    {
        foreach (Collider collider in col)
        {
            collider.enabled = true;
        }
        rb.useGravity = true;
    }

    private IEnumerator AutoCollectSequence(Action action)
    {
        setToAutoCollect = true;

        ToDisableWhileCollecting();

        // Cache the player to reduce number of accesses
        Transform cachedPlayer = GameManager._Instance.Player;

        float timeTakenToAutoCollect = 1f;
        // Move towards player position
        while (Vector3.Distance(transform.position, cachedPlayer.position) > .25f)
        {
            timeTakenToAutoCollect += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, cachedPlayer.position,
                Time.deltaTime * autoCollectSpeed * (timeTakenToAutoCollect * timeTakenToAutoCollectSpeedMultiplier));

            yield return null;
        }

        UndoDisableWhileCollecting();

        action();

        // Determine if should be added to player reserves or not
        if (RandomHelper.RandomIntExclusive(chanceToAutoCollect) <= chanceToAutoCollect.x)
        {
            OnPickup();
            ReleaseToPool();
        }
        else
        {
            ReleaseToPool();
        }
    }

    public override void ReleaseToPool()
    {
        base.ReleaseToPool();
        setToAutoCollect = false;
        expiring = false;
    }
}
