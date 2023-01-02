using UnityEngine;

public class BossHealth : EnemyHealth
{
    [HideInInspector]
    public BossBar HealthBar;

    public override void Damage(float damage, bool spawnText)
    {
        base.Damage(damage, spawnText);

        HealthBar.SetBar(currentHealth / maxHealth);
    }
}
