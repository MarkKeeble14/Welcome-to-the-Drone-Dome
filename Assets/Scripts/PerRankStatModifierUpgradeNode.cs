using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PerRankStatModifierUpgradeNode", menuName = "UpgradeNode/PerRankStatModifierUpgradeNode")]
public class PerRankStatModifierUpgradeNode : StatModifierUpgradeNode
{
    [Header("Per Rank Upgrade Node")]
    [SerializeField] private int pointsPerRank;
    [SerializeField] private int pointsToNextRank;

    public override bool Purchase()
    {
        if (Maxed()) return false;

        // Purchase right away
        purchased = true;
        OnPurchase?.Invoke();

        currentPoints++;
        pointsToNextRank--;

        // Only grow if reached a PointsPerRank breakpoint
        if (CurrentPoints % pointsPerRank != 0) return true;

        statModifier.Grow();
        pointsToNextRank = pointsPerRank;

        return true;
    }

    public override void Reset()
    {
        pointsToNextRank = pointsPerRank;
        base.Reset();
    }

    public override void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        base.SetExtraUI(nodeDisplay);

        if (nodeDisplay is InGameUpgradeNodeDisplay)
        {
            ((InGameUpgradeNodeDisplay)nodeDisplay).SetPoints(CurrentPoints, GetMaxPoints());
        }

        nodeDisplay.AddExtraText(
            "To Next Rank: " + pointsToNextRank
            + " | Change: " + (statModifier.GrowthChangeBy == StatMathOperation.ADD ? (statModifier.CurrentGrowth > 0 ? "+" : "") : "x") + statModifier.CurrentGrowth.ToString());
    }
}
