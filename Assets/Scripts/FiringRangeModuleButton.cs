using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FiringRangeModuleButton : MonoBehaviour
{
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private TextMeshProUGUI text;
    private bool purchased;
    public void SetPurchased(bool value)
    {
        purchased = value;
    }

    public void Set(ModuleType type, Action onAdd, Action onDelete)
    {
        text.text = EnumToStringHelper.GetStringValue(type);
        purchaseButton.onClick.AddListener(delegate
        {
            if (!purchased)
                onAdd();
        });
        deleteButton.onClick.AddListener(delegate
        {
            if (purchased)
                onDelete();
        });
    }

    private void Update()
    {
        deleteButton.gameObject.SetActive(purchased);
    }
}