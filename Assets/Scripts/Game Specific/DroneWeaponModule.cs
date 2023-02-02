using System.Collections;
using UnityEngine;

public abstract class DroneWeaponModule : DroneModule
{
    [Header("Base Weapon Module")]
    [SerializeField] private bool startAttackOnStart = true;
    [SerializeField] protected WeaponTargetingType targetBy;
    protected virtual WeaponTargetingType TargetBy
    {
        get { return targetBy; }
    }
    protected bool activeWhenScavenging;
    private bool active;
    protected bool paused;
    public bool Paused
    {
        get { return paused; }
        set { paused = value; }
    }

    protected DroneAttackTargeting targeting;
    protected Transform target;
    protected LayerMask enemyLayer;
    public override ModuleCategory Category => ModuleCategory.WEAPON;

    protected new void Awake()
    {
        base.Awake();

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

    public void OnEnterScavengeMode()
    {
        if (activeWhenScavenging) return;
        StopAttack();
    }

    public void OnEnterAttackMode()
    {
        if (active) return;
        StartAttack();
    }

    public void OnAdd()
    {
        StartAttack();
    }

    protected void StartAttack()
    {
        StartCoroutine(Attack());
        active = true;
    }

    public abstract IEnumerator Attack();
    protected void StopAttack()
    {
        StopAllCoroutines();
        active = false;
    }
}
