using UnityEngine;

public abstract class OverChargeableUpgradeNode : UpgradeNode
{
    [Header("Over Chargeable Upgrade Node")]
    [SerializeField] protected int PointsPerOverCharge = 5;
    protected int overChargedPoints;
    public int OverChargedPoints => overChargedPoints;

    public bool HasBeenOvercharged => OverChargedPoints > 0;

    public bool CanBeOverCharged()
    {
        return Available
            && Maxed()
            && !AtMinOrMax()
            && AboveMinPointThreshold();
    }

    private bool AboveMinPointThreshold()
    {
        return GetPointsPermitted() > Mathf.Ceil(PointsPerOverCharge / 2);
    }

    public abstract bool AtMinOrMax();

    public abstract int GetPointsPermitted();

    public void OverCharge()
    {
        // Debug.Log("Added: " + GetPointsPermitted() + " Points");
        overChargedPoints += GetPointsPermitted();
    }

    public override void Reset()
    {
        base.Reset();
        overChargedPoints = 0;
    }
}
