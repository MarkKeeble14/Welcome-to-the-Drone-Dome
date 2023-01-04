using System.Collections;
using UnityEngine;

public abstract class DroneGunModule : DroneWeaponModule
{
    [SerializeField] protected Gun gun;
    private DurationBar reloadBar;

    private new void Awake()
    {
        base.Awake();

        // Create and Set the Reload Bar
        reloadBar = Instantiate(Resources.Load<DurationBar>("Prefabs/ReloadBar"), transform);
    }

    public void Set(Gun gun)
    {
        // Create new instance of gun so we don't share instances
        this.gun = Instantiate(gun);

        // Reload the Gun
        gun.Reload();

        // Start Attacking
        StartAttack();
    }

    public override void StartAttack()
    {
        StartCoroutine(Shoot());
    }

    private float Fire()
    {
        return gun.Shoot(transform.position, target, Type);
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            if (gun.CurrentMagazineCount > 0)
            {
                // Active
                if ((target = targeting.GetTarget(gun.Range, TargetBy)) != null)
                {
                    yield return new WaitForSeconds(Fire());
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                reloadBar.gameObject.SetActive(true);
                reloadBar.HardSetBar(0);
                float reloadTime = gun.Reload();
                reloadBar.Set(reloadTime);

                // Needs to Reload
                yield return new WaitForSeconds(reloadTime);

                reloadBar.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, gun.Range);
    }
}
