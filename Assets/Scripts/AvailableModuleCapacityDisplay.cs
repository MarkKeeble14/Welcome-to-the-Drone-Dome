using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AvailableModuleCapacityDisplay : MonoBehaviour
{
    private ShopManager cachedInstance;
    [SerializeField] private TextMeshProUGUI text;
    private void Start()
    {
        cachedInstance = ShopManager._Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Capacity: " + cachedInstance.CurrentNumberOfAvailableModules + "/" + cachedInstance.MaxAvailableModules;
    }
}
