using UnityEngine;

public abstract class DroneActiveModule : DroneModule
{
    public override ModuleCategory Category => ModuleCategory.ACTIVE;

    public bool CoolingDown => cooldown < cooldownStart.Value;

    [SerializeField] protected StatModifier cooldownStart;
    public float CooldownStart => cooldownStart.Value;
    private float cooldown;
    public float CurrentCooldown
    {
        get
        {
            if (cooldown < cooldownStart.Value)
            {
                return cooldown;
            }
            else
            {
                return cooldownStart.Value;
            }
        }
    }

    public void ResetCooldown()
    {
        // Set cooldown
        cooldown = cooldownStart.Value;
    }

    protected void Awake()
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
