using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameUpgradeNodeDisplay : UpgradeNodeDisplay
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Button overChargeButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Image alterColorOf;
    private string lockedText = "Locked";
    private string unlockText = "Unlock";

    [Header("Colors")]
    [SerializeField] private Color unavailableColor;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color partiallyPurchasedColor;
    [SerializeField] private Color fullyPurchasedColor;
    [SerializeField] private Color overChargedColor;
    [SerializeField] private Color maxedColor;
    [SerializeField] private Color newlyUnlockedBackgroundColor;
    [SerializeField] private Color defaultBackgroundColor;

    private OverChargeableUpgradeNode repOverChargeable;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip unlockClip;
    [SerializeField] private AudioClip overchargeClip;

    private bool GetUnlockeable(UpgradeNode node)
    {
        return ShopManager._Instance.AllowModuleUnlock && node.Available;
    }

    private void Update()
    {
        // Set whether or not the player can over charge the node or not
        if (ShopManager._Instance.AllowModuleOverCharge && repOverChargeable != null && repOverChargeable.CanBeOverCharged())
        {
            overChargeButton.gameObject.SetActive(true);
        }
        else
        {
            overChargeButton.gameObject.SetActive(false);
        }
        // Set Unlock Button to be Active if Node is Locked
        unlockButton.gameObject.SetActive(node.Locked);
        if (GetUnlockeable(node))
        {
            unlockButtonText.text = unlockText;
        }
        else
        {
            unlockButtonText.text = lockedText;
            if (node.Requirements.Length > 0)
            {
                string s = "Requirements:";
                foreach (UpgradeNode requiredNode in node.Requirements)
                {
                    s += "\n" + requiredNode.ShortLabel;
                }
                requirementsText.text = s;
            }
        }

        // Set other aspects of UI
        SetColor(node);

        // Set Label
        SetLabel(node.ShortLabel);

        // Set any Extra UI
        node.SetExtraUI(this);
    }

    private void SetUnlock()
    {
        unlockButton.onClick.AddListener(delegate
        {
            if (GetUnlockeable(node))
            {
                // Unlock Node
                node.Unlock();
                ShopManager._Instance.UseModuleUpgradeUnlocker();

                // Audio
                sfxSource.PlayOneShot(unlockClip);
            }
            else
            {
                Debug.Log("No Module Unlockers Available");
            }
        });
    }

    private void SetOverCharge(OverChargeableUpgradeNode node)
    {
        overChargeButton.onClick.AddListener(delegate
        {
            // Over Charge Node
            node.OverCharge();
            ShopManager._Instance.UseModuleUpgradeOverCharger();

            // Audio
            sfxSource.PlayOneShot(overchargeClip);
        });
    }

    private void SetColor(UpgradeNode node)
    {
        // Change Color
        if (node.Available)
        {
            if (repOverChargeable != null && repOverChargeable.HasBeenUnlocked)
            {
                if (node.Maxed())
                {
                    alterColorOf.color = maxedColor;
                }
                else
                {
                    alterColorOf.color = overChargedColor;
                }
            }
            else if (node.Purchased)
            {
                if (node.Maxed())
                {
                    alterColorOf.color = fullyPurchasedColor;
                }
                else
                {
                    alterColorOf.color = partiallyPurchasedColor;
                }
            }
            else
            {
                alterColorOf.color = availableColor;
            }
        }
        else
        {
            alterColorOf.color = unavailableColor;
        }
    }

    public override void Set(ref UpgradeNode node, Action<UpgradeNode> onPurchase)
    {
        this.node = node;
        if (node is OverChargeableUpgradeNode)
        {
            repOverChargeable = (OverChargeableUpgradeNode)node;
            SetOverCharge(repOverChargeable);
        }

        purchaseButton.onClick.AddListener(delegate
        {
            onPurchase(this.node);
        });

        if (node.Locked)
            SetUnlock();

        backgroundImage.color = node.NewlyUnlocked ? newlyUnlockedBackgroundColor : defaultBackgroundColor;
    }

    public void SetPoints(int currentPoints, int maxPoints)
    {
        pointsText.gameObject.SetActive(true);
        pointsText.text = currentPoints + "/" + maxPoints.ToString();
    }
}
