using System.Collections;
using UnityEngine;

public abstract class DroneGunModule : DroneWeaponModule
{
    [SerializeField] protected Gun gun;
    [SerializeField] private DurationBar reloadBar;

    private new void Awake()
    {
        base.Awake();

        // Create and Set the Reload Bar
        reloadBar = Instantiate(reloadBar, transform);

        // Create new instance of gun so we don't share instances
        gun = Instantiate(gun);

        // Reload the Gun
        gun.Reload();
    }

    private float Fire()
    {
        return gun.Shoot(transform.position, target, Type);
    }

    public override IEnumerator Attack()
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
