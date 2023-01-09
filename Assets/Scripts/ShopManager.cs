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

    [Header("Currencies")]
    [SerializeField] private int startingPlayerResource;
    [SerializeField] private int currentPlayerResource;
    public int CurrentPlayerResource => currentPlayerResource;
    [SerializeField] private int freePurchases;
    public int FreePurchasesRemaining => freePurchases;
    public bool FreePurchase
    {
        get { return freePurchases > 0; }
    }
    [SerializeField] private int numberOfStartingModules = 3;
    [SerializeField] private int maxAvailableModules = 5;
    public int MaxAvailableModules => maxAvailableModules;
    [SerializeField] private int moduleUpgradeUnlockers;
    public int NumModuleUnlockers => moduleUpgradeUnlockers;
    public bool AllowUnlockModuleUpgrade => moduleUpgradeUnlockers > 0;

    [SerializeField] private bool preventDuplicateBeginningOfferings;

    [Header("Modules")]
    [SerializeField] private DroneWeaponModuleChoice weaponModuleChoicePrefab;
    [SerializeField] private Transform weaponModuleChoiceListParent;
    [SerializeField] private Dictionary<ModuleType, int> moduleCostDictionary = new Dictionary<ModuleType, int>();

    private ModuleType[] availableModules;
    private int numberOfAvailableModules
    {
        get
        {
            int x = 0;
            foreach (ModuleType type in availableModules)
            {
                if (moduleCostDictionary.ContainsKey(type))
                    x++;
            }
            return x;
        }
    }
    private int costOfAllAvailableModules
    {
        get
        {
            int x = 0;
            foreach (ModuleType type in availableModules)
            {
                if (moduleCostDictionary.ContainsKey(type))
                    x += moduleCostDictionary[type];
            }
            return x;
        }
    }
    private int numModulesActive;
    public int NumModulesActive
    {
        get { return numModulesActive; }
        set { numModulesActive = value; }
    }
    public int CurrentCapacity
    {
        get
        {
            return NumModulesActive + numberOfAvailableModules;
        }
    }
    public bool AllowModuleDrops
    {
        get
        {
            return (CurrentCapacity <= maxAvailableModules) || GameManager._Instance.OnMainMenu;
        }
    }

    public void UseModuleUpgradeUnlocker()
    {
        moduleUpgradeUnlockers--;
    }


    // Some modifier that makes resources less likely to drop when the player has a ton
    public float ResourceDropRateModifier
    {
        get
        {
            if (currentPlayerResource > 500)
            {
                return MathHelper.Normalize(currentPlayerResource, 500, costOfAllAvailableModules, 1.25f, .25f);
            }
            else
            {
                return 2;
            }
        }
    }
    private DroneWeaponModuleChoice[] spawnedDroneModuleChoices;
    [SerializeField] private TextMeshProUGUI arenaShopHelperText;

    private void Start()
    {
        SetModuleCostDictionary();

        ClearAvailableModules();
    }

    private void SetModuleCostDictionary()
    {
        List<SerializableKeyValuePair<ModuleType, DroneModuleInfo>> moduleTypeInfo = GameManager._Instance.ModuleTypeInfo.ToList();
        foreach (SerializableKeyValuePair<ModuleType, DroneModuleInfo> kvp in moduleTypeInfo)
        {
            moduleCostDictionary.Add(kvp.Key, kvp.Value.Cost);
        }
    }

    public void AddRandomModulesToAvailableModules()
    {
        List<ModuleType> possibleOptions = GameManager._Instance.AllModules;
        for (int i = 0; i < numberOfStartingModules; i++)
        {
            ModuleType type = RandomHelper.GetRandomFromList(possibleOptions);
            PickedUpModule(type);
            if (preventDuplicateBeginningOfferings)
                possibleOptions.Remove(type);
        }
    }

    public void PickedUpModule(ModuleType type)
    {
        for (int i = 0; i < availableModules.Length; i++)
        {
            if (availableModules[i] == ModuleType.NULL)
            {
                availableModules[i] = type;
                break;
            }
        }
    }

    public void RemoveModuleAtPosition(int index)
    {
        // Remove
        RemoveAtIndex(index);
    }

    public void ResetCollectables()
    {
        currentPlayerResource = startingPlayerResource;
        ClearAvailableModules();
    }

    private void ClearAvailableModules()
    {
        availableModules = new ModuleType[maxAvailableModules];
        for (int i = 0; i < availableModules.Length; i++)
        {
            availableModules[i] = ModuleType.NULL;
        }
    }

    public void AlterResource(int value)
    {
        // Add XP
        currentPlayerResource += value;
    }

    private void AddModuleDisplay(ModuleType type, int index)
    {
        DroneWeaponModuleChoice spawned = Instantiate(weaponModuleChoicePrefab, weaponModuleChoiceListParent);
        spawnedDroneModuleChoices[index] = spawned;
        spawned.Set(type, GetDroneModuleCost(type), index);
    }

    private void ClearModuleChoiceUI()
    {
        if (spawnedDroneModuleChoices == null) return;
        foreach (DroneWeaponModuleChoice spawned in spawnedDroneModuleChoices)
        {
            if (spawned == null) continue;
            Destroy(spawned.gameObject);
        }
        spawnedDroneModuleChoices = null;

    }

    private void GenerateModuleChoiceUI()
    {
        // Remove Pre-existing modules
        ClearModuleChoiceUI();

        spawnedDroneModuleChoices = new DroneWeaponModuleChoice[maxAvailableModules];

        // Spawn new ones
        for (int i = 0; i < availableModules.Length; i++)
        {
            ModuleType type = availableModules[i];
            if (type != ModuleType.NULL)
                AddModuleDisplay(type, i);
        }
    }

    private void RemoveAtIndex(int index)
    {
        availableModules[index] = ModuleType.NULL;
        DroneWeaponModuleChoice toRemove = spawnedDroneModuleChoices[index];
        Destroy(toRemove.gameObject);
        spawnedDroneModuleChoices[index] = null;
    }

    public bool PurchaseWeaponModule(ModuleType type, int purchaseCost, int index)
    {
        bool didBuy = GameManager._Instance.TryAddModule(type, purchaseCost, FreePurchase) != null;
        if (didBuy)
            RemoveAtIndex(index);
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


    public void OpenShop()
    {
        // Pause Game
        PauseManager._Instance.Pause();

        // UI
        UIManager._Instance.OpenShopUI();

        // Generate Weapon Module Choices
        GenerateModuleChoiceUI();
    }

    private int GetDroneModuleCost(ModuleType type)
    {
        return moduleCostDictionary[type];
    }

    public void UseFreePurchase()
    {
        freePurchases -= 1;
    }

    public void GiveModuleUpgradeUnlocker()
    {
        moduleUpgradeUnlockers++;
    }
}
