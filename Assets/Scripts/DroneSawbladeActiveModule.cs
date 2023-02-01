using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSawbladeActiveModule : DroneActiveModule
{
    [SerializeField] private LoadStatModifierInfo damage;
    [SerializeField] private LoadStatModifierInfo tickSpeed;
    [SerializeField] private LoadStatModifierInfo range;
    [SerializeField] private LoadStatModifierInfo maxTimeAlive;
    [SerializeField] private LoadStatModifierInfo speed;
    [SerializeField] private LoadStatModifierInfo size;
    [SerializeField] private LoadStatModifierInfo numBlades;

    [SerializeField] private Transform projectileOrigin;
    public override ModuleType Type => ModuleType.SAWBLADE_ACTIVE;

    [Header("Audio")]
    [SerializeField] private AudioClip sawbladesClip;

    public override void Effect()
    {
        // Audio
        sfxSource.PlayOneShot(sawbladesClip);

        Vector3 direction = transform.forward;

        float angleBetweenProjectiles = 360 / numBlades.Stat.Value;
        for (int i = 0; i < numBlades.Stat.Value; i++)
        {
            Sawblade sawblade = ObjectPooler.sawBladePool.Get();
            sawblade.ReleaseAction = () =>
            {
                ObjectPooler.sawBladePool.Release(sawblade);
            };
            sawblade.transform.position = projectileOrigin.position;
            sawblade.Set(damage.Stat.Value, tickSpeed.Stat.Value, size.Stat.Value, range.Stat.Value, maxTimeAlive.Stat.Value, speed.Stat.Value, direction);
            direction = Quaternion.AngleAxis(angleBetweenProjectiles, Vector3.up) * direction;
        }
    }

    protected override void LoadModuleData()
    {
        base.LoadModuleData();
        damage.SetStat(UpgradeNode.GetStatModifierUpgradeNode(damage, allModuleUpgradeNodes));
        tickSpeed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(tickSpeed, allModuleUpgradeNodes));
        range.SetStat(UpgradeNode.GetStatModifierUpgradeNode(range, allModuleUpgradeNodes));
        speed.SetStat(UpgradeNode.GetStatModifierUpgradeNode(speed, allModuleUpgradeNodes));
        size.SetStat(UpgradeNode.GetStatModifierUpgradeNode(size, allModuleUpgradeNodes));
        maxTimeAlive.SetStat(UpgradeNode.GetStatModifierUpgradeNode(maxTimeAlive, allModuleUpgradeNodes));
        numBlades.SetStat(UpgradeNode.GetStatModifierUpgradeNode(numBlades, allModuleUpgradeNodes));
    }
}
