using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Scavengeable
{
    [SerializeField] private Vector2 minMaxValue = new Vector2(1, 3);
    private int Value => RandomHelper.RandomIntInclusive(minMaxValue);
    [SerializeField] private float autoCollectSpeed = 5f;
    [SerializeField] private Vector2 chanceToAutoCollect = new Vector2(1, 4);
    [SerializeField] private float timeTakenToAutoCollectSpeedMultiplier = 2f;
    private float timeTakenToAutoCollect = 1;
    private bool setToAutoCollect;

    public override void OnPickup()
    {
        GameManager._Instance.AddResource(Value);
        Destroy(gameObject);
    }

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
