using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public abstract class UpgradeNodeDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI extraText;
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected Button purchaseButton;
    [SerializeField] protected UpgradeNode node;

    public UpgradeNode GetNode()
    {
        return node;
    }

    public abstract void Set(ref UpgradeNode node, Action<UpgradeNode> onPurchase);


    public void SetLabel(string label)
    {
        labelText.text = label;
    }

    public void AddExtraText(string text)
    {
        extraText.gameObject.SetActive(true);
        extraText.text = text;
    }
}