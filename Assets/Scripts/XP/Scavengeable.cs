using System;
using UnityEngine;

public abstract class Scavengeable : MonoBehaviour
{
    [SerializeField] protected SimpleObjectType objectPoolerPrefabKey;

    [Header("Audio")]
    [SerializeField] private AudioClip onPickupClip;

    private Action OnPickup;

    private void Awake()
    {
        OnPickup += () => AudioManager._Instance.PlayClip(onPickupClip, RandomHelper.RandomFloat(.5f, 1.4f), transform.position);
        OnPickup += PickupScavengeable;
    }

    public void Pickup()
    {
        OnPickup();
        ReleaseToPool();
    }

    public abstract void PickupScavengeable();

    public virtual void ReleaseToPool()
    {
        ObjectPooler._Instance.ReleaseSimpleObject(objectPoolerPrefabKey, gameObject);
    }
}
