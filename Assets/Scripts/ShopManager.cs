using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public bool FreePurchase
    {
        get { return freePurchases > 0; }
    }
    [Header("Modules")]
    [SerializeField] private DroneWeaponModuleChoice weaponModuleChoicePrefab;
    [SerializeField] private Transform weaponModuleChoiceListParent;
    [SerializeField]
    private SerializableDictionary<ModuleType, int> weaponModuleBaseCostDictionary
        = new SerializableDictionary<ModuleType, int>();

    private List<ModuleType> availableModules = new List<ModuleType>();
    [SerializeField] private int maxAvailableModules = 5;
    public int CurrentNumberOfAvailableModules => availableModules.Count;
    public int MaxAvailableModules => maxAvailableModules;
    public bool AllowModuleDrops => availableModules.Count < maxAvailableModules;
    // Some modifier that makes resources more likely to drop when needed
    public float ResourceDropRateModifier => CurrentNumberOfAvailableModules;
    public void PickedUpModule(ModuleType type)
    {
        availableModules.Add(type);
    }

    public void ResetCollectables()
    {
        availableModules.Clear();
        currentPlayerResource = startingPlayerResource;
    }

    public void AlterResource(int value)
    {
        // Add XP
        currentPlayerResource += value;
    }

    private void Start()
    {
        AlterResource(startingPlayerResource);
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
        return weaponModuleBaseCostDictionary.GetEntry(type).Value;
    }

    public bool PurchaseWeaponModule(ModuleType type, int purchaseCost)
    {
        bool didBuy = GameManager._Instance.TryPurchaseModule(type, purchaseCost, FreePurchase);
        if (didBuy)
            availableModules.Remove(type);
        return didBuy;
    }

    public void CloseShop()
    {
        // Resume Game
        PauseManager._Instance.Resume();

        // UI
        UIManager._Instance.CloseShopUI();
        UIManager._Instance.OpenInGameUI();
    }
}
