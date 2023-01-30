using System.Collections;
using UnityEngine;

public abstract class DroneGunModule : DroneWeaponModule
{
    [Header("Upgradeables")]
    [SerializeField] protected LoadStatModifierInfo shotsPerSecond;
    [SerializeField] protected LoadStatModifierInfo range;
    [Header("Stats")]
    [SerializeField] protected bool infiniteAmmo = true;
    [SerializeField] protected int maxMagazineCount = 10;
    protected int currentMagazineCount;
    [SerializeField] protected float reloadTime = 2f;
    private float preventWaitCancelingTimer;

    [Header("References")]
    [SerializeField] private DurationBar reloadBar;
    [SerializeField] private Transform projectileOrigin;
    [SerializeField] private Transform reloadBarHolder;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;

    public int CurrentMagazineCount
    {
        get { return currentMagazineCount; }
    }

    protected new void Awake()
    {
        base.Awake();

        // Create and Set the Reload Bar
        reloadBar = Instantiate(reloadBar, reloadBarHolder);
        reloadBar.HardSetBar(1);
        reloadBar.SetText(EnumToStringHelper.GetStringValue(Type) + "\nReloading");

        // Reload the Gun
        Reload();
    }

    private void Update()
    {
        if (preventWaitCancelingTimer > 0)
            preventWaitCancelingTimer -= Time.deltaTime;
        else
            preventWaitCancelingTimer = 0;
    }

    protected override void LoadModuleData()
    {
        shotsPerSecond.SetStat(UpgradeNode.GetStatModifierUpgradeNode(shotsPerSecond, allModuleUpgradeNodes));
        range.SetStat(UpgradeNode.GetStatModifierUpgradeNode(range, allModuleUpgradeNodes));
    }

    public float Reload()
    {
        currentMagazineCount = maxMagazineCount;
        return reloadTime;
    }

    public float Reload(int projectiles)
    {
        if (currentMagazineCount + projectiles > maxMagazineCount)
            currentMagazineCount = maxMagazineCount;
        else
            currentMagazineCount += projectiles;
        return reloadTime;
    }

    // Returns the cooldown betewen next shot
    public virtual float Shoot(Vector3 projectileOrigin, Transform shootAt, ModuleType source)
    {
        // Audio 
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(shootClip);

        // Remove a projectile from the guns magazine unless the gun has infinite ammo
        if (!infiniteAmmo)
        {
            // Debug.Log(currentMagazineCount);
            currentMagazineCount--;
        }

        return (1 / shotsPerSecond.Stat.Value);
    }

    public void ResetStats()
    {
        range.Stat.Reset();
    }

    private float Fire()
    {
        return Shoot(projectileOrigin.position, target, Type);
    }

    public override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(preventWaitCancelingTimer);

            if (!Attached)
            {
                continue;
            }

            // Active
            if ((target = targeting.GetTarget(range.Stat.Value, projectileOrigin, TargetBy)) != null)
            {
                float nextShotTime = Fire();

                if (CurrentMagazineCount > 0)
                {
                    preventWaitCancelingTimer = nextShotTime;
                    yield return new WaitForSeconds(nextShotTime);
                }
                else
                {
                    float reloadTime = Reload();

                    reloadBar.SetText(EnumToStringHelper.GetStringValue(Type) + "\nReloading");
                    reloadBar.Set(reloadTime);

                    // Needs to Reload
                    preventWaitCancelingTimer = reloadTime;
                    yield return new WaitForSeconds(reloadTime);
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
