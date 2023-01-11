﻿using System.Collections;
using UnityEngine;

public abstract class DroneGunModule : DroneWeaponModule
{

    [Header("Upgradeables")]
    [SerializeField] protected LoadStatModifierInfo shotsPerSecond;
    [SerializeField] protected LoadStatModifierInfo range;
    [Header("Stats")]
    [SerializeField] protected bool infiniteAmmo = true;
    [SerializeField] protected int maxMagazineCount = 10;
    protected int currentMagazineCount;
    [SerializeField] protected float reloadTime = 2f;

    [Header("References")]
    [SerializeField] private DurationBar reloadBar;

    public int CurrentMagazineCount
    {
        get { return currentMagazineCount; }
    }

    protected new void Awake()
    {
        base.Awake();

        // Create and Set the Reload Bar
        reloadBar = Instantiate(reloadBar, transform);
        reloadBar.SetText("Reloading");
        reloadBar.HardSetBar(1);

        // Reload the Gun
        Reload();
    }

    protected override void LoadModuleData()
    {
        shotsPerSecond.SetStat(UpgradeNode.GetStatModifierUpgradeNode(shotsPerSecond, allModuleUpgradeNodes));
        range.SetStat(UpgradeNode.GetStatModifierUpgradeNode(range, allModuleUpgradeNodes));
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

        return (1 / shotsPerSecond.Stat.Value);
    }

    public void ResetStats()
    {
        range.Stat.Reset();
    }

    private float Fire()
    {
        return Shoot(transform.position, target, Type);
    }

    public override IEnumerator Attack()
    {
        while (true)
        {
            if (CurrentMagazineCount > 0)
            {
                // Active
                if ((target = targeting.GetTarget(range.Stat.Value, transform, TargetBy)) != null)
                {
                    yield return new WaitForSeconds(Fire());
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                float reloadTime = Reload();

                reloadBar.SetText("Reloading");
                reloadBar.Set(reloadTime);

                // Needs to Reload
                yield return new WaitForSeconds(reloadTime);
            }
        }
    }
}
