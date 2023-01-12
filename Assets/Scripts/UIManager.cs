using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }
    private DronesDisplay currentDronesDisplay;
    public DronesDisplay CurrentDroneDisplay
    {
        get { return currentDronesDisplay; }
        private set { currentDronesDisplay = value; }
    }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [Header("Drone Displays")]
    [SerializeField] private DronesDisplay mainMenuRegularDroneDisplay;
    [SerializeField] private DronesDisplay mainMenuFiringRangeDroneDisplay;
    [SerializeField] private DronesDisplay inGameDroneDisplay;
    [SerializeField] private DronesDisplay shopDroneDisplay;

    public void SetCurrentDronesDisplayForMenu()
    {
        currentDronesDisplay = mainMenuRegularDroneDisplay;
    }

    public void SetCurrentDronesDisplayForFiringRange()
    {
        currentDronesDisplay = mainMenuFiringRangeDroneDisplay;
    }

    public void OpenInGameUI()
    {
        mainMenuUI.SetActive(false);

        inGameUI.SetActive(true);
        shopUI.SetActive(false);
        upgradeUI.SetActive(false);

        // Set other UI
        currentDronesDisplay = inGameDroneDisplay;
        inGameDroneDisplay.Set();
    }

    public void OpenShopUI()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(true);
        upgradeUI.SetActive(false);

        // Set other UI
        currentDronesDisplay = shopDroneDisplay;
        shopDroneDisplay.Set();
    }

    public void OpenUpgradeUI()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(false);
        upgradeUI.SetActive(true);
    }

    public void OpenWinScreen()
    {
        CloseShopUI();
        CloseUpgradeUI();
        CloseInGameUI();

        winScreen.SetActive(true);
    }

    public void OpenLoseScreen()
    {
        CloseShopUI();
        CloseUpgradeUI();
        CloseInGameUI();

        loseScreen.SetActive(true);
    }

    public void CloseInGameUI()
    {
        inGameUI.SetActive(false);
    }

    public void CloseShopUI()
    {
        shopUI.SetActive(false);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
    }
}
