using UnityEngine;

public abstract class OverChargeableUpgradeNode : UpgradeNode
{
    [Header("Over Chargeable Upgrade Node")]
    protected int overChargedPoints;
    public int OverChargedPoints => overChargedPoints;
    [SerializeField] protected int PointsPerOverCharge = 5;

    public bool HasBeenUnlocked => OverChargedPoints > 0;

    public virtual bool CanBeOverCharged()
    {
        return Available && Maxed();
    }

    public void OverCharge()
    {
        overChargedPoints += PointsPerOverCharge;
    }

    public override void Reset()
    {
        base.Reset();
        overChargedPoints = 0;
    }
}
