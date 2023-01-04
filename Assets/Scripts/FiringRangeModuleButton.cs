using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FiringRangeModuleButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    public void Set(ModuleType type, Action action)
    {
        text.text = type.ToString();
        button.onClick.AddListener(delegate { action(); });
    }
}