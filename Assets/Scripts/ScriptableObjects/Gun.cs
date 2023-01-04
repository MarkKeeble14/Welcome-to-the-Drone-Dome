using System;
using UnityEngine;

public abstract class Gun : ScriptableObject
{
    [Header("Base")]
    [SerializeField] protected StatModifier shotsPerSecond;
    [SerializeField] protected bool infiniteAmmo;
    [SerializeField] protected int maxMagazineCount;
    protected int currentMagazineCount;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected StatModifier range;
    public float Range => range.Value;

    public int CurrentMagazineCount
    {
        get { return currentMagazineCount; }
    }

    public float Reload()
    {
        currentMagazineCount = maxMagazineCount;
        return reloadTime;
    }

    public float Reload(int projectiles)
    {
        if (currentMagazineCount + projectiles > maxMagazineCount)
            currentMagazineCount = maxMagazineCount;
        else
            currentMagazineCount += projectiles;
        return reloadTime;
    }

    // Returns the cooldown betewen next shot
    public virtual float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        // Remove a projectile from the guns magazine unless the gun has infinite ammo
        if (!infiniteAmmo)
        {
            // Debug.Log(currentMagazineCount);
            currentMagazineCount--;
        }

        return (1 / shotsPerSecond.Value);
    }

    public void ResetStats()
    {
        range.Reset();
    }
}
