using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "StatModifierUpgradeNode", menuName = "UpgradeNode/StatModifierUpgradeNode")]
public class StatModifierUpgradeNode : UpgradeNode
{
    public int CurrentPoints;
    public int MaxPoints;
    public GrowthStatModifier statModifier;
    public Action OnPurchase;

    public override bool Maxed()
    {
        return CurrentPoints >= MaxPoints;
    }

    public override void Purchase()
    {
        if (Maxed()) return;

        OnPurchase?.Invoke();
        statModifier.Grow();
        Purchased = true;
        CurrentPoints++;
    }

    public override void Reset()
    {
        base.Reset();
        CurrentPoints = 0;
        statModifier.Reset();
    }

    public override void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        base.SetExtraUI(nodeDisplay);
        nodeDisplay.SetPoints(CurrentPoints, MaxPoints);
        nodeDisplay.AddExtraText("Change: " + (statModifier.CurrentGrowth > 0 ? "+" : "") + statModifier.CurrentGrowth.ToString());
    }
}
