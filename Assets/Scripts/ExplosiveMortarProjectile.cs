using System;
using System.Collections;
using UnityEngine;

public class ExplosiveMortarProjectile : MortarProjectile
{
    [Header("Explosive Mortar Projectile")]
    [SerializeField] private ModuleExplodeable mainExplodeable;
    [SerializeField] private ModuleExplodeable toDrop;

    public override void ArrivedAtPosition()
    {
        mainExplodeable.CallExplode(false);
        ReleaseAction?.Invoke();
    }

    public void Set(StatModifier mainDamage, StatModifier mainRadius, StatModifier mainPower, StatModifier mainLift,
        BoolSwitchUpgradeNode shouldDrop, StatModifier timeBetweenDrops,
        StatModifier droppedDamage, StatModifier droppedRadius, StatModifier droppedPower, StatModifier droppedLift)
    {
        // Set main explodeable values
        mainExplodeable.SetExplosionData(mainDamage.Value, mainRadius.Value, mainPower.Value, mainLift.Value);

        // If player has unlocked a specific upgrade node, we drop explosives
        if (shouldDrop.Active)
            StartCoroutine(DropExplosives(timeBetweenDrops, droppedDamage, droppedRadius, droppedPower, droppedLift));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DropExplosives(StatModifier timeBetweenDrops, StatModifier damage, StatModifier radius, StatModifier power, StatModifier lift)
    {
        yield return new WaitForSeconds(timeBetweenDrops.Value);

        ModuleExplodeable ex = Instantiate(toDrop, transform.position, Quaternion.identity);
        ex.SetExplosionData(damage.Value, radius.Value, power.Value, lift.Value);

        StartCoroutine(DropExplosives(timeBetweenDrops, damage, power, radius, lift));
    }
}
