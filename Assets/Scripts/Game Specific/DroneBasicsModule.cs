using System.Collections.Generic;
using UnityEngine;

public class DroneBasicsModule : DronePassiveModule
{
    [Header("Upgradeables")]
    [SerializeField] private LoadStatModifierInfo moveSpeed;
    public StatModifierUpgradeNode MoveSpeed => moveSpeed.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo shoveStrength;
    public StatModifierUpgradeNode ShoveStrength => shoveStrength.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo scavengeableSightRange;
    public StatModifierUpgradeNode ScavengeableSightRange => scavengeableSightRange.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo scavengingGrabRange;
    public StatModifierUpgradeNode ScavengingGrabRange => scavengingGrabRange.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo scavengingSpeedMod;
    public StatModifierUpgradeNode ScavengingSpeedMod => scavengingSpeedMod.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo orbitSpeed;
    public StatModifierUpgradeNode OrbitSpeed => orbitSpeed.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo orbitDistance;
    public StatModifierUpgradeNode OrbitDistance => orbitDistance.UpgradeNode;

    public override ModuleType Type => ModuleType.DRONE_BASICS;

    protected override void LoadModuleData()
    {
        moveSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(moveSpeed, allModuleUpgradeNodes));
        shoveStrength.SetStat(UpgradeNode.GetStatModifierUpgradeNode(shoveStrength, allModuleUpgradeNodes));
        scavengeableSightRange.SetStat(UpgradeNode.GetStatModifierUpgradeNode(scavengeableSightRange, allModuleUpgradeNodes));
        scavengingGrabRange.SetStat(UpgradeNode.GetStatModifierUpgradeNode(scavengingGrabRange, allModuleUpgradeNodes));
        scavengingSpeedMod.SetStat(UpgradeNode.GetStatModifierUpgradeNode(scavengingSpeedMod, allModuleUpgradeNodes));
        orbitSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(orbitSpeed, allModuleUpgradeNodes));
        orbitDistance.SetStat(UpgradeNode.GetStatModifierUpgradeNode(orbitDistance, allModuleUpgradeNodes));
    }
}
