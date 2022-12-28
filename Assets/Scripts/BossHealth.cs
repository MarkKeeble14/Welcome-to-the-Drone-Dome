using UnityEngine;

public class BossHealth : EnemyHealth
{
    [HideInInspector]
    public BossBar HealthBar;

    public override void Damage(float damage)
    {
        base.Damage(damage);

        HealthBar.SetBar(currentHealth / startHealth);
    }
}
