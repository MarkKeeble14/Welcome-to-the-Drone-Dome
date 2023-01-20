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

    public void Set(DroneModule module, Action action, bool interactable)
    {
        if (interactable)
        {
            button.interactable = true;
            SetNew(module.HasNewlyUnlockedNode);
            button.onClick.AddListener(delegate { action(); });
        }
        else
        {
            button.interactable = false;
        }
        typeText.text = EnumToStringHelper.GetStringValue(module.Type);
    }
}
