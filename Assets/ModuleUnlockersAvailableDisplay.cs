using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModuleUnlockersAvailableDisplay : MonoBehaviour
{
    private ShopManager cachedInstance;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "Module Unlockers: ";
    private void Start()
    {
        cachedInstance = ShopManager._Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = prefix + cachedInstance.NumModuleUnlockers.ToString();
    }
}
