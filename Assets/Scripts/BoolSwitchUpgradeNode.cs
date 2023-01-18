using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolSwitchUpgradeNode", menuName = "UpgradeNode/BoolSwitchUpgradeNode")]
public class BoolSwitchUpgradeNode : UpgradeNode
{
    [Header("Bool Switch Upgrade Node")]
    [SerializeField] private bool startState;
    private bool active;
    public bool Active => active;

    [SerializeField] private int numPointsToBePurchased = 1;
    public int NumPointsToBePurchased => numPointsToBePurchased;
    [SerializeField] private int numPointsToBeActive = 1;
    public int NumPointsToBeActive => numPointsToBeActive;
    public Action OnBecomeActive;

    public override int GetMaxPoints()
    {
        return NumPointsToBeActive;
    }

    public override bool Purchase()
    {
        // If already maxed, don't worry about changing anything
        if (Maxed()) return false;

        // Increment the number of points invested
        currentPoints++;

        if (CurrentPoints >= NumPointsToBePurchased)
            purchased = true;
        if (CurrentPoints >= NumPointsToBeActive)
        {
            OnBecomeActive?.Invoke();
            active = true;
        }
        return true;
    }

    public override void Reset()
    {
        base.Reset();

        currentPoints = 0;
        active = startState;
    }

    public override void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        base.SetExtraUI(nodeDisplay);
        nodeDisplay.SetPoints(CurrentPoints, NumPointsToBeActive);
    }

    public override string GetStatState()
    {
        return Active ? "On" : "Off";
    }
}
