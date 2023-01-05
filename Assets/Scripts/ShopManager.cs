using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [Header("Player Resource")]
    [SerializeField] private int startingPlayerResource;
    [SerializeField] private int currentPlayerResource;
    public int CurrentPlayerResource => currentPlayerResource;
    [SerializeField] private int freePurchases;
    public int FreePurchasesRemaining => freePurchases;
    public bool FreePurchase
    {
        get { return freePurchases > 0; }
    }
    [Header("Modules")]
    [SerializeField] private DroneWeaponModuleChoice weaponModuleChoicePrefab;
    [SerializeField] private Transform weaponModuleChoiceListParent;
    [SerializeField] private Dictionary<ModuleType, int> moduleCostDictionary = new Dictionary<ModuleType, int>();
    [SerializeField] private int numberOfStartingModules = 3;
    private List<ModuleType> availableModules = new List<ModuleType>();
    private int costOfAllAvailableModules
    {
        get
        {
            int x = 0;
            foreach (ModuleType type in availableModules)
            {
                x += moduleCostDictionary[type];
            }
            return x;
        }
    }
    [SerializeField] private int maxAvailableModules = 5;
    public int CurrentNumberOfAvailableModules => availableModules.Count;
    public int MaxAvailableModules => maxAvailableModules;
    public bool AllowModuleDrops => availableModules.Count < maxAvailableModules;
    // Some modifier that makes resources less likely to drop when the player has a ton
    public float ResourceDropRateModifier
    {
        get
        {
            if (currentPlayerResource > 500)
            {
                return MathHelper.Normalize(currentPlayerResource, 500, costOfAllAvailableModules, 1.5f, .25f);
            }
            else
            {
                return 3;
            }
        }
    }
    [SerializeField] private TextMeshProUGUI arenaShopHelperText;


    public void PickedUpModule(ModuleType type)
    {
        availableModules.Add(type);
    }

    public void ResetCollectables()
    {
        currentPlayerResource = startingPlayerResource;
        availableModules.Clear();
        AddRandomModulesToAvailableModules();
    }

    public void AlterResource(int value)
    {
        // Add XP
        currentPlayerResource += value;
    }

    private void Start()
    {
        SetModuleCostDictionary();
    }

    public void AddRandomModulesToAvailableModules()
    {
        for (int i = 0; i < numberOfStartingModules; i++)
        {
            ModuleType type = RandomHelper.GetRandomFromList(GameManager._Instance.AllModules);
            PickedUpModule(type);
        }
    }

    private void SetModuleCostDictionary()
    {
        List<SerializableKeyValuePair<ModuleType, DroneModuleInfo>> moduleTypeInfo = GameManager._Instance.ModuleTypeInfo.ToList();
        foreach (SerializableKeyValuePair<ModuleType, DroneModuleInfo> kvp in moduleTypeInfo)
        {
            moduleCostDictionary.Add(kvp.Key, kvp.Value.Cost);
        }
    }

    public void UseFreePurchase()
    {
        freePurchases -= 1;
    }

    public void OpenShop()
    {
        // Pause Game
        PauseManager._Instance.Pause();

        // UI
        UIManager._Instance.OpenShopUI();

        // Generate Weapon Module Choices
        GenerateModuleChoices();
    }

    private void GenerateModuleChoices()
    {
        foreach (ModuleType type in availableModules)
        {
            DroneWeaponModuleChoice spawned = Instantiate(weaponModuleChoicePrefab, weaponModuleChoiceListParent);
            spawned.Set(type, GetDroneModuleCost(type));
        }
    }

    private int GetDroneModuleCost(ModuleType type)
    {
        return moduleCostDictionary[type];
    }

    public bool PurchaseWeaponModule(ModuleType type, int purchaseCost)
    {
        bool didBuy = GameManager._Instance.TryAddModule(type, purchaseCost, FreePurchase) != null;
        if (didBuy)
            availableModules.Remove(type);
        return didBuy;
    }

    public void CloseShop()
    {
        arenaShopHelperText.gameObject.SetActive(false);

        // Resume Game
        PauseManager._Instance.Resume();

        // UI
        UIManager._Instance.CloseShopUI();
        UIManager._Instance.OpenInGameUI();
    }
}
