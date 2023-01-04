using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolSwitchUpgradeNode", menuName = "UpgradeNode/BoolSwitchUpgradeNode")]
public class BoolSwitchUpgradeNode : UpgradeNode
{
    public bool Active;
    public int CurrentPoints;
    public int NumPointsToBePurchased = 1;
    public int NumPointsToBeActive = 1;
    public Action OnBecomeActive;

    public override bool Maxed()
    {
        return CurrentPoints >= NumPointsToBeActive;
    }

    public override void Purchase()
    {
        // If already maxed, don't worry about changing anything
        if (Maxed()) return;

        // Increment the number of points invested
        CurrentPoints++;

        if (CurrentPoints >= NumPointsToBePurchased)
            Purchased = true;
        if (CurrentPoints >= NumPointsToBeActive)
        {
            OnBecomeActive?.Invoke();
            Active = true;
        }
    }

    public override void Reset()
    {
        base.Reset();

        CurrentPoints = 0;
        Active = false;
    }

    public override void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        base.SetExtraUI(nodeDisplay);
        nodeDisplay.SetPoints(CurrentPoints, NumPointsToBeActive);
    }
}
