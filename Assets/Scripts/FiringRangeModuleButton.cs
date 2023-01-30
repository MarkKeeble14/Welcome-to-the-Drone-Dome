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

    [Header("Audio")]
    [SerializeField] private AudioClip purchaseClip;
    [SerializeField] private AudioClip discardClip;
    [SerializeField] private AudioClip failPurchaseClip;

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
            {
                onAdd();

                // Audio
                AudioManager._Instance.PlayClip(purchaseClip, true);
            }
            else
            {
                // Audio
                AudioManager._Instance.PlayClip(failPurchaseClip, true);
            }
        });
        deleteButton.onClick.AddListener(delegate
        {
            if (purchased)
            {
                onDelete();

                // Audio
                AudioManager._Instance.PlayClip(discardClip, true);
            }
        });
    }

    private void Update()
    {
        deleteButton.gameObject.SetActive(purchased);
    }
}