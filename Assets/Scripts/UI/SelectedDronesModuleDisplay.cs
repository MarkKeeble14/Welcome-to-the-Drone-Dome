using UnityEngine;
using TMPro;

public class SelectedDronesModuleDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText;
    public void Set(ModuleType type, ModuleCategory category)
    {
        typeText.text = type.ToString();
    }
}
