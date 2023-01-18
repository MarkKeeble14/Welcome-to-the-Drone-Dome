using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SelectedDronesModuleDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Button button;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color newBackgroundColor;
    [SerializeField] private Color defaultBackgroundColor;

    public void SetNew(bool v)
    {
        backgroundImage.color = v ? newBackgroundColor : defaultBackgroundColor;
    }

    public void Set(DroneModule module, Action action)
    {
        SetNew(module.HasNewlyUnlockedNode);
        typeText.text = EnumToStringHelper.GetStringValue(module.Type);
        button.onClick.AddListener(delegate { action(); });
    }
}
