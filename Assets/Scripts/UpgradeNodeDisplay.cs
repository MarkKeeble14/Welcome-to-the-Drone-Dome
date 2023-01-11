using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UpgradeNodeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI extraText;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image alterColorOf;
    [SerializeField] private RectTransform rt;

    [SerializeField] private Color unavailableColor;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color partiallyPurchasedColor;
    [SerializeField] private Color fullyPurchasedColor;
    [SerializeField] private Color unlockedColor;

    [SerializeField] private UpgradeNode node;
    private UnlockableUpgradeNode repUnlockable;

    public void Set(ref UpgradeNode node)
    {

        this.node = node;
        if (node is UnlockableUpgradeNode)
        {
            repUnlockable = (UnlockableUpgradeNode)node;
            SetUnlock(repUnlockable);
        }
        SetOnClick();
    }

    private void Update()
    {
        // Set whether or not the player can unlock the node or not
        if (ShopManager._Instance.AllowUnlockModuleUpgrade && repUnlockable != null && repUnlockable.CanBeUnlocked())
        {
            unlockButton.gameObject.SetActive(true);
        }
        else
        {
            unlockButton.gameObject.SetActive(false);
        }
        // Set other aspects of UI
        SetColor(node);
        SetLabel(node.Label);
        node.SetExtraUI(this);
    }

    private void SetUnlock(UnlockableUpgradeNode node)
    {
        unlockButton.onClick.AddListener(delegate
        {
            // Unlock Node
            node.Unlock();
            ShopManager._Instance.UseModuleUpgradeUnlocker();
        });
    }

    private void SetColor(UpgradeNode node)
    {
        if (node.Available)
        {
            if (repUnlockable != null && repUnlockable.HasBeenUnlocked)
            {
                alterColorOf.color = unlockedColor;
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

    private void SetOnClick()
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