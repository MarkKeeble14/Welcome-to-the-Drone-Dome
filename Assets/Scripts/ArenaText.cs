using UnityEngine;
using TMPro;
using System;

public class ArenaText : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private TextMeshProUGUI text;
    public void SetText(string text)
    {
        this.text.gameObject.SetActive(true);
        this.text.text = prefix + text;
    }

    internal void Hide()
    {
        text.gameObject.SetActive(false);
    }
}
