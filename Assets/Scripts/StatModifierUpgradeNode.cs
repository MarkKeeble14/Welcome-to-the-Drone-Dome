using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "StatModifierUpgradeNode", menuName = "UpgradeNode/StatModifierUpgradeNode")]
public class StatModifierUpgradeNode : UpgradeNode
{
    public int CurrentPoints;
    public int MaxPoints;
    public GrowthStatModifier statModifier;

    public override bool Maxed()
    {
        return CurrentPoints >= MaxPoints;
    }

    public override void Purchase()
    {
        if (Maxed()) return;

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
    }
}
