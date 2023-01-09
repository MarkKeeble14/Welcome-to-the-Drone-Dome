using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScavengeable : AutoCollectScavengeable
{
    [SerializeField] private float healAmount = 1f;

    public override void OnPickup()
    {
        GameManager._Instance.HealPlayer(healAmount);
        ReleaseToPool();
    }
}
