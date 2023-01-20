using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PermaUpgradeNodeDisplay : UpgradeNodeDisplay
{
    private IUpgradeNodePermanantelyUpgradeable repStat;
    public override void Set(ref UpgradeNode node, Action<UpgradeNode> onPurchase)
    {
        this.node = node;

        repStat = (IUpgradeNodePermanantelyUpgradeable)node;

        purchaseButton.onClick.AddListener(delegate
        {
            onPurchase(this.node);
        });
    }

    private void Update()
    {
        SetLabel(repStat.GetLabel());
        AddExtraText(repStat.GetToShow());
    }
}
