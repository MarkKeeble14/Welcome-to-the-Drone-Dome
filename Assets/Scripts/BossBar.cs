using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BossBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Bar bar;

    public void SetBar(float v)
    {
        bar.SetBar(v);
    }

    public void Set(GameObject gameObject)
    {
        text.text = gameObject.name;
    }
}
