using System.Collections;
using UnityEngine;

public abstract class DroneWeaponModule : DroneModule
{
    [Header("Base Attack Module")]
    [SerializeField] private bool startAttackOnStart = true;
    protected WeaponTargetingType targetBy;
    protected virtual WeaponTargetingType TargetBy
    {
        get { return targetBy; }
    }
    [SerializeField] protected DroneAttackTargeting targeting;
    protected Transform target;
    protected LayerMask enemyLayer;
    public override ModuleCategory Category => ModuleCategory.WEAPON;

    protected void Awake()
    {
        // Get Attack Targeting Component from Drone
        targeting = transform.parent.GetComponent<DroneAttackTargeting>();

        // Set Enemy Layer
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected void Start()
    {
        if (startAttackOnStart)
        {
            StartAttack();
        }
    }

    public void StartAttack()
    {
        StartCoroutine(Attack());
    }

    public abstract IEnumerator Attack();
    public void StopAttack()
    {
        StopAllCoroutines();
    }
}
