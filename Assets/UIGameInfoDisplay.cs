using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGameInfoDisplay : MonoBehaviour
{
    [SerializeField] private DisplayStat stat;
    [SerializeField] private string prefix;
    [SerializeField] private string suffix;

    private TextMeshProUGUI text;
    private ShopManager cachedShop;
    private UpgradeManager cachedUpgrade;

    private void Start()
    {
        cachedShop = ShopManager._Instance;
        cachedUpgrade = UpgradeManager._Instance;
        text = GetComponent<TextMeshProUGUI>();
    }

    private string GetText()
    {
        switch (stat)
        {
            case DisplayStat.SHOP_MANAGER_OVER_CHARGERS:
                return cachedShop.NumModuleOverChargers.ToString();
            case DisplayStat.SHOP_MANAGER_UNLOCKERS:
                return cachedShop.NumModuleUnlockers.ToString();
            case DisplayStat.SHOP_MANAGER_RESOURCES:
                return cachedShop.CurrentPlayerResource.ToString();
            case DisplayStat.UPGRADE_MANAGER_UPGRADE_POINTS:
                return cachedUpgrade.UpgradePointsAvailable.ToString();
            case DisplayStat.SHOP_MANAGER_CAPACITY:
                return cachedShop.CurrentCapacity.ToString() + "/" + cachedShop.MaxAvailableModules.ToString();
            case DisplayStat.SHOP_MANAGER_FREE_PURCHASES:
                return cachedShop.FreePurchasesRemaining.ToString();
            default:
                return "Uncaught Type";
        }
    }

    // Update is called once per frame
    void Update()
    {
        text.text = prefix + GetText() + suffix;
    }
}
