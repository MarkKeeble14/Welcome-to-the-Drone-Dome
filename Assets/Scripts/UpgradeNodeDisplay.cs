using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeNodeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private Button button;
    [SerializeField] private Image alterColorOf;

    [SerializeField] private Color unavailableColor;
    [SerializeField] private Color availableColor;
    [SerializeField] private Color partiallyPurchasedColor;
    [SerializeField] private Color fullyPurchasedColor;

    public void Set(UpgradeNode node, Action action)
    {
        SetLabel(node.Label);
        SetColor(node);
        SetOnClick(action);
        node.SetExtraUI(this);
    }

    private void SetColor(UpgradeNode node)
    {
        if (node.Available)
        {
            if (node.Purchased)
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

    private void SetOnClick(Action action)
    {
        button.onClick.AddListener(delegate { action(); });
    }

    public void SetLabel(string label)
    {
        labelText.text = label;
    }

    public void SetPoints(int currentPoints, int maxPoints)
    {
        pointsText.gameObject.SetActive(true);
        pointsText.text = currentPoints + "/" + maxPoints;
    }

    public void AddExtraText(string text)
    {
        pointsText.text += "\n" + text;
    }
}