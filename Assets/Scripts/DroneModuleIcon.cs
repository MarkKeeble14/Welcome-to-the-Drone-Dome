using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneModuleIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    public void Set(Sprite sprite, Color color)
    {
        image.sprite = sprite;
        image.color = color;
    }
}
