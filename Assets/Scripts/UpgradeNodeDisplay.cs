using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UpgradeNodeDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI extraText;
    [SerializeField] private TextMeshProUGUI unlockButtonText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button overChargeButton;
    [SerializeField] private Button purchaseButton;
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
    [SerializeField] private Color newBackgroundColor;
    [SerializeField] private Color defaultBackgroundColor;


    [SerializeField] private UpgradeNode node;
    private OverChargeableUpgradeNode repOverChargeable;
    public UpgradeNode GetNode()
    {
        return node;
    }

    public void Set(ref UpgradeNode node)
    {
        this.node = node;
        if (node is OverChargeableUpgradeNode)
        {
            repOverChargeable = (OverChargeableUpgradeNode)node;
            SetOverCharge(repOverChargeable);
        }
        SetOnPurchase();

        if (node.Locked)
            SetUnlock();

        backgroundImage.color = node.NewlyUnlocked ? newBackgroundColor : defaultBackgroundColor;
    }

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
        unlockButtonText.text = GetUnlockeable(node) ? unlockText : lockedText;

        // Set other aspects of UI
        SetColor(node);

        // Set Label
        SetLabel(node.Label);

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
        });
    }

    private void SetColor(UpgradeNode node)
    {
        // Change Color
        if (node.Available)
        {
            if (repOverChargeable != null && repOverChargeable.HasBeenUnlocked)
            {
                alterColorOf.color = overChargedColor;
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

    private void SetOnPurchase()
    {
        purchaseButton.onClick.AddListener(delegate
        {
            // Debug.Log("OnClick Instance ID: " + node.GetInstanceID() + ", " + ((StatModifierUpgradeNode)node).Stat.GetInstanceID());
            UpgradeManager._Instance.TryPurchaseNode(node);
        });
    }

    public void SetLabel(string label)
    {
        labelText.text = label;
    }

    public void SetPoints(int currentPoints, int maxPoints)
    {
        pointsText.gameObject.SetActive(true);
        pointsText.text = currentPoints + "/" + maxPoints.ToString();
    }

    public void AddExtraText(string text)
    {
        extraText.gameObject.SetActive(true);
        extraText.text = text;
    }
}