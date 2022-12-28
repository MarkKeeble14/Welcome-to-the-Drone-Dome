using System.Collections;
using UnityEngine;

public class DroneGunModule : DroneAttackModule
{
    [SerializeField] protected Gun gun;

    private new void Start()
    {
        base.Start();
    }

    public void Set(Gun gun)
    {
        // Create new instance of gun so we don't share instances
        this.gun = Instantiate(gun);

        // Start Attacking
        StartAttack();
    }

    public override void StartAttack()
    {
        StartCoroutine(Shoot());
    }

    private float Fire()
    {
        return gun.Shoot(transform.position, target);
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
                // Needs to Reload
                yield return new WaitForSeconds(gun.Reload());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, gun.Range);
    }
}
