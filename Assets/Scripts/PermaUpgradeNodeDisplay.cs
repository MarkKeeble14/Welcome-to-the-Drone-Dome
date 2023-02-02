using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PermaUpgradeNodeDisplay : UpgradeNodeDisplay
{
    private IUpgradeNodePermanantelyUpgradeable repStat;

    [Header("Audio")]
    [SerializeField] private AudioClip purchaseClip;
    [SerializeField] private AudioClip failPurchaseClip;

    public override void Set(ref UpgradeNode node, Action<UpgradeNode> onPurchase)
    {
        this.node = node;

        repStat = (IUpgradeNodePermanantelyUpgradeable)node;
        string pre = repStat.GetToShow();

        purchaseButton.onClick.AddListener(delegate
        {
            if (!repStat.CanUpgradePermanantly())
            {
                AudioManager._Instance.PlayClip(failPurchaseClip, true);
                return;
            }
            onPurchase(this.node);

            // If pre does not equal post, purchase was successful
            if (!pre.Equals(repStat.GetToShow()))
            {
                // Audio
                AudioManager._Instance.PlayClip(purchaseClip, true);
            }
            else
            {
                // Audio
                AudioManager._Instance.PlayClip(failPurchaseClip, true);
            }
        });
    }

    private void Update()
    {
        SetLabel(repStat.GetLabel());
        AddExtraText(repStat.GetToShow());
    }
}
