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
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
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

    [SerializeField] private int maxWeaponModuleChoices;

    [SerializeField]
    private SerializableDictionary<ModuleType, int> weaponModuleBaseCostDictionary
        = new SerializableDictionary<ModuleType, int>();
    [SerializeField]
    private SerializableDictionary<ModuleType, int> maxModuleInstanceDictionary
        = new SerializableDictionary<ModuleType, int>();
    private List<ModuleType> limitedAvailabilityModules = new List<ModuleType>();
    [SerializeField]
    private List<ModuleType> possibleWeaponModules;

    private void Start()
    {
        limitedAvailabilityModules.AddRange(maxModuleInstanceDictionary.Keys());
    }

    public void OpenShop()
    {
        // Pause Game
        PauseManager._Instance.Pause();

        // UI
        UIManager._Instance.OpenShopUI();

        // Generate Weapon Module Choices
        GenerateWeaponModuleChoices();
    }

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

    private int GetDroneWeaponModuleCost(ModuleType type)
    {
        return weaponModuleBaseCostDictionary.GetEntry(type).Value;
    }

    public bool PurchaseWeaponModule(ModuleType type, int purchaseCost)
    {
        offered.Remove(type);
        if (FreePurchase)
        {
            freePurchases -= 1;
            return GameManager._Instance.AddModule(type, purchaseCost, true);
        }
        else
        {
            return GameManager._Instance.AddModule(type, purchaseCost, false);
        }
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
