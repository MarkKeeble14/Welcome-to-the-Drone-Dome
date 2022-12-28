using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void SetBar(float percentage)
    {
        fill.fillAmount = percentage;
    }
}
