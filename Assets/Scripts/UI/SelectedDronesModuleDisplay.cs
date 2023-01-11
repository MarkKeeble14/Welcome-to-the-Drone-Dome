using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SelectedDronesModuleDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Button button;
    public void Set(DroneModule module, Action action)
    {
        typeText.text = ModuleTypeStringValues.GetStringValue(module.Type);
        button.onClick.AddListener(delegate { action(); });
    }
}
