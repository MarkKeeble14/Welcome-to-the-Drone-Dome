using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AvailableModuleCapacityDisplay : MonoBehaviour
{
    private ShopManager cachedInstance;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "Module Capacity: ";
    private void Start()
    {
        cachedInstance = ShopManager._Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = prefix + cachedInstance.CurrentCapacity + "/" + cachedInstance.MaxAvailableModules;
    }
}
