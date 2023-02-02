using System;
using UnityEngine;

public abstract class Scavengeable : MonoBehaviour
{
    private GameObject releaseable;
    protected Action OnPickup;
    [SerializeField] protected SimpleObjectType objectPoolerPrefabKey;

    [Header("Audio")]
    [SerializeField] private AudioClip onPickupClip;

    protected virtual void OnEnable()
    {
        // Tell Object what to Release
        SetReleaseable(gameObject);
    }

    private void Awake()
    {
        OnPickup += () => AudioManager._Instance.PlayClip(onPickupClip, RandomHelper.RandomFloat(.5f, 1.4f), transform.position, .8f);
        OnPickup += ReleaseToPool;
    }

    public virtual void PickupScavengeable()
    {
        OnPickup();
    }

    public virtual void ReleaseToPool()
    {
        ObjectPooler._Instance.ReleaseSimpleObject(objectPoolerPrefabKey, releaseable);
    }

    protected void SetReleaseable(GameObject obj)
    {
        releaseable = obj;
    }
}
