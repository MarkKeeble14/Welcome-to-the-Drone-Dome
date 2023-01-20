using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreIntDisplay : MonoBehaviour
{
    [SerializeField] private StoreInt display;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix;
    [SerializeField] private string suffix;

    // Update is called once per frame
    void Update()
    {
        text.text = prefix + display.Value.ToString() + suffix;
    }
}
