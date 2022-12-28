using UnityEngine;

public abstract class DroneAttackModule : DroneModule
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

    private void Awake()
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
