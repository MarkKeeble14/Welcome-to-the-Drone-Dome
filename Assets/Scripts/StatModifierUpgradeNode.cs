﻿using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "StatModifierUpgradeNode", menuName = "UpgradeNode/StatModifierUpgradeNode")]
public class StatModifierUpgradeNode : OverChargeableUpgradeNode
{
    [Header("Stat Modifier Upgrade Node")]
    [SerializeField] protected int maxPoints = 5;
    public int MaxPoints => maxPoints;
    [SerializeField] protected GrowthStatModifier1 statModifier;
    public StatModifier1 Stat => statModifier;
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
        nodeDisplay.SetPoints(CurrentPoints, GetMaxPoints());
        nodeDisplay.AddExtraText("Change: " + (statModifier.GrowthChangeBy == StatMathOperation.ADD ? (statModifier.CurrentGrowth > 0 ? "+" : "") : "x")
                + statModifier.CurrentGrowth.ToString());
    }

    public override string GetStatState()
    {
        return statModifier.Value.ToString();
    }
}
