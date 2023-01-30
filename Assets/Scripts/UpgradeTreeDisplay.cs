using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeTreeDisplay : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;


    [Header("Audio")]
    [SerializeField] private AudioClip clickClip;

    public void Set(UpgradeTree tree, Action onClick)
    {
        // Debug.Log("Set: ");
        UpgradeTreeDisplayInfo info = GameManager._Instance.GetUpgradeTreeDisplayInfo(tree.UpgradeTreeRelation);
        image.sprite = info.Sprite;
        image.color = info.Color;
        text.text = info.Text;
        button.onClick.AddListener(delegate
        {
            // Audio
            AudioManager._Instance.PlayClip(clickClip, true);

            // Debug.Log("Clicked: " + name);
            onClick();
        });
    }
}
