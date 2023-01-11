using UnityEngine;

public class HelpPlayerMovementModule : DronePassiveModule
{
    [Header("Upgradeables")]
    [SerializeField] private LoadStatModifierInfo moveSpeed;
    public StatModifierUpgradeNode MoveSpeed => moveSpeed.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo dashSpeed;
    public StatModifierUpgradeNode DashSpeed => dashSpeed.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo dashDuration;
    public StatModifierUpgradeNode DashDuration => dashDuration.UpgradeNode;
    [SerializeField] private LoadStatModifierInfo dashCooldown;
    public StatModifierUpgradeNode DashCooldown => dashCooldown.UpgradeNode;

    public override ModuleType Type => ModuleType.HELP_PLAYER_MOVEMENT;

    protected override void LoadModuleData()
    {
        moveSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(moveSpeed, allModuleUpgradeNodes));
        dashSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(dashSpeed, allModuleUpgradeNodes));
        dashDuration.SetStat(UpgradeNode.GetStatModifierUpgradeNode(dashDuration, allModuleUpgradeNodes));
        dashCooldown.SetStat(UpgradeNode.GetStatModifierUpgradeNode(dashCooldown, allModuleUpgradeNodes));
    }
}
