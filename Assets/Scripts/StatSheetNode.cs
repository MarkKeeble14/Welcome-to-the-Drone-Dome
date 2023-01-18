using UnityEngine;
using TMPro;

public class StatSheetNode : MonoBehaviour
{
    [SerializeField] private UpgradeNode node;
    [SerializeField] private TextMeshProUGUI text;
    public void Set(UpgradeNode node)
    {
        this.node = node;
    }

    private void Update()
    {
        text.text = node.ShortLabel + ": " + node.GetStatState();
    }
}