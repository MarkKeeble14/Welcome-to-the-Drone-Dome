using UnityEngine;

public abstract class UnlockableUpgradeNode : UpgradeNode
{
    [Header("Unlockable Upgrade Node")]
    protected int unlockedPoints;
    public int UnlockedPoints => unlockedPoints;
    [SerializeField] protected int PointsPerUnlock = 5;

    public bool HasBeenUnlocked => UnlockedPoints > 0;

    public virtual bool CanBeUnlocked()
    {
        return Available && Maxed();
    }

    public void Unlock()
    {
        unlockedPoints += PointsPerUnlock;
    }

    public override void Reset()
    {
        base.Reset();
        unlockedPoints = 0;
    }
}
