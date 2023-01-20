using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "StatModifierUpgradeNode", menuName = "UpgradeNode/StatModifierUpgradeNode")]
public class StatModifierUpgradeNode : OverChargeableUpgradeNode, IUpgradeNodePermanantelyUpgradeable
{
    [Header("Stat Modifier Upgrade Node")]
    [SerializeField] protected int maxPoints = 5;
    public int MaxPoints => maxPoints;
    [SerializeField] protected GrowthStatModifier statModifier;
    public StatModifier Stat => statModifier;
    public Action OnPurchase;

    public override int GetMaxPoints()
    {
        return MaxPoints + OverChargedPoints;
    }

    public override bool Purchase()
    {
        if (Maxed()) return false;

        OnPurchase?.Invoke();
        statModifier.Grow();
        purchased = true;
        currentPoints++;
        return true;
    }

    public override void Reset()
    {
        base.Reset();
        currentPoints = 0;
        statModifier.Reset();
    }

    public override void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        base.SetExtraUI(nodeDisplay);
        if (nodeDisplay is InGameUpgradeNodeDisplay)
        {
            ((InGameUpgradeNodeDisplay)nodeDisplay).SetPoints(CurrentPoints, GetMaxPoints());
        }
        nodeDisplay.AddExtraText("Change: " + (statModifier.GrowthChangeBy == StatMathOperation.ADD ? (statModifier.CurrentGrowth > 0 ? "+" : "") : "x")
                + statModifier.CurrentGrowth.ToString());
    }

    public override string GetStatState()
    {
        return statModifier.Value.ToString();
    }

    public void Upgrade()
    {
        ++statModifier.numTimesBaseUpgraded;
    }

    public void HardReset()
    {
        statModifier.numTimesBaseUpgraded = 0;
    }

    public string GetLabel()
    {
        return Label;
    }

    public string GetToShow()
    {
        return "Value: " + Math.Round(statModifier.Value, 2).ToString();
    }
}
