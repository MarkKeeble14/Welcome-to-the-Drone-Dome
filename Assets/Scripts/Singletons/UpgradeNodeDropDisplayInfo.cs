using UnityEngine;

[System.Serializable]
public class UpgradeTreeDisplayInfo
{
    [SerializeField] private Color color;
    public Color Color => color;
    [SerializeField] private string text;
    public string Text => text;
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    public UpgradeTreeDisplayInfo(Color color, string text, Sprite sprite)
    {
        this.color = color;
        this.text = text;
        this.sprite = sprite;
    }
}