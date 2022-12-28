using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Scavengeable
{
    [SerializeField] private int value = 1;

    public override void OnPickup()
    {
        GameManager._Instance.AddResource(value);
        Destroy(gameObject);
    }
}
