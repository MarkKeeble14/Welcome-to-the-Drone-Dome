using UnityEngine;

public abstract class DroneActiveModule : DroneModule
{
    public override ModuleCategory Category => ModuleCategory.ACTIVE;

    public bool CoolingDown => cooldown < cooldownStart.Stat.Value;

    [SerializeField] private LoadStatModifierInfo cooldownStart;
    public float CooldownStart => cooldownStart.Stat.Value;
    private float cooldown;
    public float CurrentCooldown
    {
        get
        {
            if (cooldown < cooldownStart.Stat.Value)
            {
                return cooldown;
            }
            else
            {
                return cooldownStart.Stat.Value;
            }
        }
    }

    protected override void LoadModuleData()
    {
        cooldownStart.SetStat(UpgradeNode.GetStatModifierUpgradeNode(cooldownStart, allModuleUpgradeNodes));
    }

    public void ResetCooldown()
    {
        // Set cooldown
        cooldown = cooldownStart.Stat.Value;
    }

    protected void Start()
    {
        ResetCooldown();
    }

    public void Activate()
    {
        if (CoolingDown) return;
        Effect();
        cooldown = 0;
    }

    public abstract void Effect();

    private void Update()
    {
        if (CoolingDown)
            cooldown += Time.deltaTime;
    }
}
