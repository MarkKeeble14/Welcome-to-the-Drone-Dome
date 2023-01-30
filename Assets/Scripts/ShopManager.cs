using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

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
    [Header("Money")]
    [SerializeField] private int startingPlayerResource;
    [SerializeField] private int currentPlayerResource;
    public int CurrentPlayerResource => currentPlayerResource;

    [Header("Free Purchases")]
    [SerializeField] private int freePurchases;
    public int FreePurchasesRemaining => freePurchases;
    public bool FreePurchase
    {
        get { return freePurchases > 0; }
    }


    [Header("Other Purchases")]
    [Header("Upgrade Point")]
    [SerializeField] private int baseUpgradePointCost;
    [SerializeField] private int baseUpgradePointCostGrowth;
    private int upgradePointCost;
    [SerializeField] private float upgradePointCostGrowth;

    [Header("Module Unlocker")]
    [SerializeField] private int moduleUnlockerCost;
    [SerializeField] private float moduleUnlockerCostGrowth;

    [Header("Module Overcharger")]
    [SerializeField] private int moduleOverchargerCost;
    [SerializeField] private float moduleOverchargerCostGrowth;

    [Header("Module Over Chargers")]
    [SerializeField] private int moduleUpgradeOverChargers;
    public int NumModuleOverChargers => moduleUpgradeOverChargers;
    public bool AllowModuleOverCharge => moduleUpgradeOverChargers > 0;

    [Header("Module Unlockers")]
    [SerializeField] private int startingModuleUnlockers;
    [SerializeField] private int moduleUpgradeUnlockers;
    public int NumModuleUnlockers => moduleUpgradeUnlockers;
    public bool AllowModuleUnlock => moduleUpgradeUnlockers > 0;

    [SerializeField] private int numberOfStartingModules = 3;
    [SerializeField] private int maxAvailableModules = 5;
    public int MaxAvailableModules => maxAvailableModules;
    [SerializeField] private bool preventDuplicateBeginningOfferings;

    [Header("Module Info")]
    [SerializeField] private DroneWeaponModuleChoice weaponModuleChoicePrefab;
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

    [Header("Module Slots")]
    [SerializeField] private int baseActiveSlotCost = 750;
    [SerializeField] private int basePassiveSlotCost = 750;
    [SerializeField] private int baseWeaponSlotCost = 500;
    [SerializeField] private float activeSlotCostGrowth = 500f;
    [SerializeField] private float passiveSlotCostGrowth = 250f;
    [SerializeField] private float weaponSlotCostGrowth = 400f;
    private int activeSlotCost;
    private int passiveSlotCost;
    private int weaponSlotCost;
    [SerializeField] private Image activeSlotButton;
    [SerializeField] private TextMeshProUGUI activeSlotButtonText;
    [SerializeField] private Image passiveSlotButton;
    [SerializeField] private TextMeshProUGUI passiveSlotButtonText;
    [SerializeField] private Image weaponSlotButton;
    [SerializeField] private TextMeshProUGUI weaponSlotButtonText;
    [SerializeField] private Color buttonDefaultColor;

    [Header("References")]
    [SerializeField] private Transform weaponModuleChoiceListParent;
    [SerializeField] private ShowSelectedDronesModulesDisplay showSelectedDronesModulesDisplay;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private TextMeshProUGUI buyExtraHelperText;
    [SerializeField] private TextMeshProUGUI upgradePointsButtonText;
    [SerializeField] private TextMeshProUGUI moduleUnlockerButtonText;
    [SerializeField] private TextMeshProUGUI moduleOVerchargerButtonText;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip openShopClip;
    [SerializeField] private AudioClip discardClip;
    [SerializeField] private AudioClip purchaseModuleClip;
    [SerializeField] private AudioClip purchaseOtherClip;
    [SerializeField] private AudioClip failPurchaseClip;

    public void ResetUpgradePointCost(bool grow)
    {
        if (grow)
            baseUpgradePointCost += baseUpgradePointCostGrowth;
        upgradePointCost = baseUpgradePointCost;
    }

    // Some modifier that makes resources less likely to drop when the player has a ton
    public float ResourceDropRateModifier
    {
        get
        {
            if (currentPlayerResource > 500)
            {
                return MathHelper.Normalize(currentPlayerResource, 500, costOfAllAvailableModules + moduleOverchargerCost + moduleUnlockerCost, 1.5f, .25f);
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

        ResetUpgradePointCost(false);

        SetModuleUnlockerText();
        SetModuleOverchargerText();
        SetUpgradePointText();
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
        SetAllPurchaseModuleSlotText();

        // Audio
        sfxSource.PlayOneShot(openShopClip);
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
        moduleUpgradeUnlockers = startingModuleUnlockers;
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
        {
            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(purchaseModuleClip);
            sfxSource.pitch = 1f;

            RemoveAtIndex(index);
            showSelectedDronesModulesDisplay.Set(playerDroneController.SelectedDrone.AppliedModules, false);
        }
        else
        {
            // Audio
            sfxSource.PlayOneShot(failPurchaseClip);
        }

        return didBuy;
    }

    private int GetDroneModuleCost(ModuleType type)
    {
        return moduleCostDictionary[type];
    }

    public void UseFreePurchase()
    {
        freePurchases -= 1;
    }

    public void UseModuleUpgradeUnlocker()
    {
        moduleUpgradeUnlockers--;
    }

    public void AddModuleUpgradeUnlocker()
    {
        moduleUpgradeUnlockers++;
    }

    public void UseModuleUpgradeOverCharger()
    {
        moduleUpgradeOverChargers--;
    }

    public void AddModuleUpgradeOverCharger()
    {
        moduleUpgradeOverChargers++;
    }

    private void SetModuleUnlockerText()
    {
        moduleUnlockerButtonText.text = "Buy Module Unlocker: $" + moduleUnlockerCost;
    }

    private void SetModuleOverchargerText()
    {
        moduleOVerchargerButtonText.text = "Buy Module Overcharger: $" + moduleOverchargerCost;
    }

    private void SetUpgradePointText()
    {
        upgradePointsButtonText.text = "Buy Upgrade Point: $" + upgradePointCost;
    }

    public void BuyUpgradePoint()
    {
        if (CurrentPlayerResource > upgradePointCost)
        {
            buyExtraHelperText.gameObject.SetActive(false);
            currentPlayerResource -= upgradePointCost;
            upgradePointCost = Mathf.RoundToInt(upgradePointCost * upgradePointCostGrowth);
            UpgradeManager._Instance.AddUpgradePoints(1);
            SetUpgradePointText();

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(purchaseOtherClip);
            sfxSource.pitch = 1f;
        }
        else
        {
            buyExtraHelperText.gameObject.SetActive(true);
            buyExtraHelperText.text = "Insufficnet Funds";

            // Audio
            sfxSource.PlayOneShot(failPurchaseClip);
        }
    }

    public void BuyModuleUnlocker()
    {
        if (CurrentPlayerResource > moduleUnlockerCost)
        {
            buyExtraHelperText.gameObject.SetActive(false);
            currentPlayerResource -= moduleUnlockerCost;
            moduleUnlockerCost = Mathf.RoundToInt(moduleUnlockerCost * moduleUnlockerCostGrowth);
            AddModuleUpgradeUnlocker();
            SetModuleUnlockerText();

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(purchaseOtherClip);
            sfxSource.pitch = 1f;
        }
        else
        {
            buyExtraHelperText.gameObject.SetActive(true);
            buyExtraHelperText.text = "Insufficnet Funds";

            // Audio
            sfxSource.PlayOneShot(failPurchaseClip);
        }
    }

    public void BuyModuleOvercharger()
    {
        if (CurrentPlayerResource > moduleOverchargerCost)
        {
            buyExtraHelperText.gameObject.SetActive(false);
            currentPlayerResource -= moduleOverchargerCost;
            moduleOverchargerCost = Mathf.RoundToInt(moduleOverchargerCost * moduleOverchargerCostGrowth);
            AddModuleUpgradeOverCharger();
            SetModuleOverchargerText();

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(purchaseOtherClip);
            sfxSource.pitch = 1f;
        }
        else
        {
            buyExtraHelperText.gameObject.SetActive(true);
            buyExtraHelperText.text = "Insufficnet Funds";

            // Audio
            sfxSource.PlayOneShot(failPurchaseClip);
        }
    }


    private void SetPurchaseModuleSlotText(TextMeshProUGUI text, int cost)
    {
        text.text = "+Slot: $" + cost;
    }

    public void SetAllPurchaseModuleSlotText()
    {
        if (playerDroneController.SelectedDrone == null) return;
        activeSlotCost = baseActiveSlotCost + Mathf.RoundToInt(playerDroneController.SelectedDrone.ActiveSlotsAdded * activeSlotCostGrowth);
        passiveSlotCost = basePassiveSlotCost + Mathf.RoundToInt(playerDroneController.SelectedDrone.PassiveSlotsAdded * passiveSlotCostGrowth);
        weaponSlotCost = baseWeaponSlotCost + Mathf.RoundToInt(playerDroneController.SelectedDrone.WeaponSlotsAdded * weaponSlotCostGrowth);
        SetPurchaseModuleSlotText(activeSlotButtonText, activeSlotCost);
        SetPurchaseModuleSlotText(passiveSlotButtonText, passiveSlotCost);
        SetPurchaseModuleSlotText(weaponSlotButtonText, weaponSlotCost);
    }

    public void TryBuyActiveSlot()
    {
        BuyModuleSlot(ModuleCategory.ACTIVE);
    }
    public void TryBuyPassiveSlot()
    {
        BuyModuleSlot(ModuleCategory.PASSIVE);
    }
    public void TryBuyWeaponSlot()
    {
        BuyModuleSlot(ModuleCategory.WEAPON);
    }

    private float flashDuration = .1f;
    public void BuyModuleSlot(ModuleCategory category)
    {
        switch (category)
        {
            case ModuleCategory.ACTIVE:
                if (CurrentPlayerResource < activeSlotCost)
                {
                    StartCoroutine(FlashImageColor(activeSlotButton, Color.red, flashDuration));

                    // Audio
                    sfxSource.PlayOneShot(failPurchaseClip);
                    return;
                }
                StartCoroutine(FlashImageColor(activeSlotButton, Color.green, flashDuration));
                currentPlayerResource -= activeSlotCost;
                playerDroneController.SelectedDrone.AddActiveSlot();

                break;
            case ModuleCategory.PASSIVE:
                if (CurrentPlayerResource < passiveSlotCost)
                {
                    StartCoroutine(FlashImageColor(passiveSlotButton, Color.red, flashDuration));
                    // Audio
                    sfxSource.PlayOneShot(failPurchaseClip);
                    return;
                }
                StartCoroutine(FlashImageColor(passiveSlotButton, Color.green, flashDuration));
                currentPlayerResource -= passiveSlotCost;
                playerDroneController.SelectedDrone.AddPassiveSlot();
                break;
            case ModuleCategory.WEAPON:
                if (CurrentPlayerResource < weaponSlotCost)
                {
                    StartCoroutine(FlashImageColor(weaponSlotButton, Color.red, flashDuration));
                    // Audio
                    sfxSource.PlayOneShot(failPurchaseClip);
                    return;
                }
                StartCoroutine(FlashImageColor(weaponSlotButton, Color.green, flashDuration));
                currentPlayerResource -= weaponSlotCost;
                playerDroneController.SelectedDrone.AddWeaponSlot();
                break;
        }
        // Update UI
        SetAllPurchaseModuleSlotText();
        showSelectedDronesModulesDisplay.Set(playerDroneController.SelectedDrone.AppliedModules, false);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(purchaseOtherClip);
        sfxSource.pitch = 1f;
    }

    private IEnumerator FlashImageColor(Image image, Color color, float duration)
    {
        image.color = color;
        yield return new WaitForSecondsRealtime(duration);
        image.color = buttonDefaultColor;
    }
}