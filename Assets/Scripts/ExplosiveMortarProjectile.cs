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

    public void Set(StatModifier1 mainDamage, StatModifier1 mainRadius, StatModifier1 mainPower, StatModifier1 mainLift,
        BoolSwitchUpgradeNode shouldDrop, StatModifier1 timeBetweenDrops,
        StatModifier1 droppedDamage, StatModifier1 droppedRadius, StatModifier1 droppedPower, StatModifier1 droppedLift)
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

    private IEnumerator DropExplosives(StatModifier1 timeBetweenDrops, StatModifier1 damage, StatModifier1 radius, StatModifier1 power, StatModifier1 lift)
    {
        yield return new WaitForSeconds(timeBetweenDrops.Value);

        ModuleExplodeable ex = Instantiate(toDrop, transform.position, Quaternion.identity);
        ex.SetExplosionData(damage.Value, radius.Value, power.Value, lift.Value);

        StartCoroutine(DropExplosives(timeBetweenDrops, damage, power, radius, lift));
    }
}
