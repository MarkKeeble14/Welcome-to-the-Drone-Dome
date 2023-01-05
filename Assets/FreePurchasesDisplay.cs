using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FreePurchasesDisplay : MonoBehaviour
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
        text.text = "Free Purchases: " + cachedInstance.FreePurchasesRemaining;
    }
}
