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
    private string baseUpgradedString = "TimesPermanantUpgraded";
    private string baseUpgradedStringKey
    {
        get
        {
            return name + baseUpgradedString;
        }
    }
    public Action OnPurchase;

    public override int GetPointsPermitted()
    {
        if (!Stat.HasMin && !Stat.HasMax)
        {
            return PointsPerOverCharge;
        }
        else
        {
            if (AtMinOrMax()) return 0;

            StatModifierUpgradeNode copy = Instantiate(this);
            copy.Stat.BaseValue = statModifier.Value;
            copy.overChargedPoints += PointsPerOverCharge;

            for (int i = 0; i < PointsPerOverCharge; i++)
            {
                if (copy.AtMinOrMax())
                {
                    return i - 1;
                }
                copy.Purchase();
            }
            return PointsPerOverCharge;
        }
    }

    public override bool AtMinOrMax()
    {
        if (statModifier.HasMin && (statModifier.Value <= statModifier.MinValue || Mathf.Approximately(statModifier.Value, statModifier.MinValue))) return true;
        if (statModifier.HasMax && (statModifier.Value >= statModifier.MaxValue || Mathf.Approximately(statModifier.Value, statModifier.MaxValue))) return true;
        return false;
    }

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
        LoadValue();
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
        if (!statModifier.HasMin && !statModifier.HasMax)
        {
            ++statModifier.numTimesBaseUpgraded;
            SaveValue();
        }
        else
        {
            if (CanUpgradePermanantly())
            {
                ++statModifier.numTimesBaseUpgraded;
                SaveValue();
            }
        }
    }

    public void HardReset()
    {
        statModifier.numTimesBaseUpgraded = 0;
        SaveValue();
    }

    public string GetLabel()
    {
        return Label;
    }

    public string GetToShow()
    {
        return "Current Value: " + Math.Round(statModifier.Value, 2).ToString() + "\nChange on Purchase: "
            + (statModifier.PermaGrowth > 0 ? "+" : "") + statModifier.PermaGrowth;
    }

    public bool CanUpgradePermanantly()
    {
        return !AtMinOrMax();
    }

    public void LoadValue()
    {
        if (PlayerPrefs.HasKey(baseUpgradedStringKey))
        {
            statModifier.numTimesBaseUpgraded = PlayerPrefs.GetInt(baseUpgradedStringKey);
        }
        else
        {
            statModifier.numTimesBaseUpgraded = 0;
        }
    }

    public void SaveValue()
    {
        PlayerPrefs.SetInt(baseUpgradedStringKey, statModifier.numTimesBaseUpgraded);
    }
}
