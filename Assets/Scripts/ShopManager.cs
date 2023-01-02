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

    [SerializeField] private int freePurchases;
    public bool FreePurchase
    {
        get { return freePurchases > 0; }
    }
    [SerializeField] private DroneWeaponModuleChoice weaponModuleChoicePrefab;
    [SerializeField] private Transform weaponModuleChoiceListParent;

    // [SerializeField] private int maxWeaponModuleChoices;

    [SerializeField]
    private SerializableDictionary<ModuleType, int> weaponModuleBaseCostDictionary
        = new SerializableDictionary<ModuleType, int>();

    /*
    [SerializeField]
    private SerializableDictionary<ModuleType, int> maxModuleInstanceDictionary
        = new SerializableDictionary<ModuleType, int>();
    private List<ModuleType> limitedAvailabilityModules = new List<ModuleType>();
    */
    private List<ModuleType> possibleWeaponModules;
    public List<ModuleType> PossibleWeaponModules => possibleWeaponModules;

    private List<ModuleType> availableModules = new List<ModuleType>();

    public void PickedUpModule(ModuleType type)
    {
        availableModules.Add(type);
    }

    private void Start()
    {
        // Create initial list of possible weapon modules, which is determined by which modules have been given costs
        possibleWeaponModules = new List<ModuleType>();
        possibleWeaponModules.AddRange(weaponModuleBaseCostDictionary.Keys());

        // Add modules which have a limited availability to the list
        // limitedAvailabilityModules.AddRange(maxModuleInstanceDictionary.Keys());
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

    /*
    Old, Random Weapon Module Types

    private List<ModuleType> offered = new List<ModuleType>();

    private void GenerateWeaponModuleChoices()
    {
        // Add previously offered weapon modules back into the pool if unpurchased
        foreach (ModuleType type in offered)
        {
            // Type is of a limited variety
            if (limitedAvailabilityModules.Contains(type))
            {
                // Debug.Log("Offered and Not Purchased Type is a Limited Availability Module");
                // Dictionary does not contain the key, meaning it at one point did, but was removed the last iteration
                if (!maxModuleInstanceDictionary.ContainsKey(type))
                {
                    // Debug.Log("Re-inserting the Type into the Dictionary");
                    // Put it back, with a capacity of 1
                    maxModuleInstanceDictionary.Set(type, 1);
                    possibleWeaponModules.Add(type);
                }
                else
                {
                    // Debug.Log("Re-incrementing the Type in the Dictionary");
                    // Dictionary already contains the key, increment it's count
                    maxModuleInstanceDictionary.Set(type, maxModuleInstanceDictionary.GetEntry(type).Value++);
                }
            }
        }
        // Clear the offered list now that we've dealt with all that
        offered.Clear();


        // Remove all old offering game objects/displays
        foreach (Transform child in weaponModuleChoiceListParent)
        {
            Destroy(child.gameObject);
        }

        // Generate new offerings
        for (int i = 0; i < maxWeaponModuleChoices; i++)
        {
            if (possibleWeaponModules.Count <= 0) break;

            // Get random weapon module type
            ModuleType randomWeaponModule
                = RandomHelper.GetRandomFromList(possibleWeaponModules);

            // Check if there is supposed to be a limited amount of that weapon module type
            if (maxModuleInstanceDictionary.ContainsKey(randomWeaponModule))
            {
                // Debug.Log("Found Type in Max Module Dictionary");

                // If so, first check the value currently stored -1; if 0, this is the last 
                // time this weapon may be offered, remove it from the list of possible options
                // if above 0, simply replace the old stored value with it's updated amount
                int newValue = maxModuleInstanceDictionary.GetEntry(randomWeaponModule).Value - 1;
                if (newValue <= 0)
                {
                    // Debug.Log("Removing Type from Options");
                    possibleWeaponModules.Remove(randomWeaponModule);
                    maxModuleInstanceDictionary.RemoveEntry(randomWeaponModule);
                }
                else
                {
                    // Debug.Log("Decrementing Availability in Max Module Dictionary");
                    maxModuleInstanceDictionary.Set(randomWeaponModule, newValue);
                }
            }

            DroneWeaponModuleChoice spawned = Instantiate(weaponModuleChoicePrefab, weaponModuleChoiceListParent);
            spawned.Set(randomWeaponModule, GetDroneWeaponModuleCost(randomWeaponModule));

            offered.Add(randomWeaponModule);
        }
    }
    */

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
