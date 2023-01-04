using UnityEngine;

public abstract class DroneWeaponModule : DroneModule
{
    [Header("Base Attack Module")]
    [SerializeField] private bool startAttackOnStart;
    protected WeaponTargetingType targetBy;
    protected virtual WeaponTargetingType TargetBy
    {
        get { return targetBy; }
    }
    protected DroneAttackTargeting targeting;
    protected Transform target;
    public override ModuleCategory Category => ModuleCategory.WEAPON;

    protected void Awake()
    {
        targeting = GetComponent<DroneAttackTargeting>();
    }

    protected void Start()
    {
        if (startAttackOnStart)
        {
            StartAttack();
        }
    }

    public abstract void StartAttack();
    public void StopAttack()
    {
        StopAllCoroutines();
    }
}
