using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTreeDisplay : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;

    public void Set(UpgradeTree tree, Action onClick)
    {
        // Debug.Log("Set: ");
        UpgradeTreeDisplayInfo info = GameManager._Instance.GetUpgradeTreeDisplayInfo(tree.UpgradeTreeRelation);
        image.sprite = info.Sprite;
        image.color = info.Color;
        button.onClick.AddListener(delegate
        {
            // Debug.Log("Clicked: " + name);
            onClick();
        });
    }
}
