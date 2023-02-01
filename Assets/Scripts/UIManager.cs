using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("High Score")]
    [SerializeField] private EnemiesKilledBar waveBar;
    [SerializeField] private TextMeshProUGUI winHighScoreText;
    [SerializeField] private TextMeshProUGUI loseHighScoreText;
    [SerializeField] private string highScoreKey = "WavesCompleted";
    [SerializeField] private string enemiesKilledKey = "EnemiesKilled";

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

        InputManager.EnablePlayerControls();

        // Set other UI
        currentDronesDisplay = inGameDroneDisplay;
        inGameDroneDisplay.Set();
    }

    public void OpenShopUI()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(true);
        upgradeUI.SetActive(false);

        InputManager.DisablePlayerControls();

        // Set other UI
        currentDronesDisplay = shopDroneDisplay;
        shopDroneDisplay.Set();
    }

    public void OpenUpgradeUI()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(false);
        upgradeUI.SetActive(true);

        InputManager.DisablePlayerControls();
    }

    public void OpenWinScreen()
    {
        CloseShopUI();
        CloseUpgradeUI();
        CloseInGameUI();

        SetHighScore(winHighScoreText);
        winScreen.SetActive(true);
    }

    public void OpenLoseScreen()
    {
        CloseShopUI();
        CloseUpgradeUI();
        CloseInGameUI();

        SetHighScore(loseHighScoreText);
        loseScreen.SetActive(true);
    }

    private void AddHighScore(TextMeshProUGUI text, string key, float value, string name, PlayerPrefsType type)
    {
        switch (type)
        {
            case PlayerPrefsType.FLOAT:
                AddFloatHighScore(text, key, value, name);
                break;
            case PlayerPrefsType.INT:
                AddIntHighScore(text, key, Mathf.RoundToInt(value), name);
                break;
            default:
                break;
        }
    }

    private void AddIntHighScore(TextMeshProUGUI text, string key, int value, string name)
    {
        // High Score
        string hsString = name + value;
        if (PlayerPrefs.HasKey(key))
        {
            int hsValue = PlayerPrefs.GetInt(key);
            Debug.Log(hsValue + ", " + value);
            if (value > hsValue)
            {
                hsString += "\nNew High Score!: " + value;
                PlayerPrefs.SetInt(key, value);
            }
            else
            {
                hsString += "\nHigh Score: " + hsValue;
            }
        }
        else
        {
            PlayerPrefs.SetInt(key, value);
        }
        text.text += (text.text.Equals("") ? "" : "\n") + hsString;
    }

    private void AddFloatHighScore(TextMeshProUGUI text, string key, float value, string name)
    {
        // High Score
        string hsString = name + value;
        if (PlayerPrefs.HasKey(key))
        {
            float hsValue = PlayerPrefs.GetFloat(key);
            if (value > hsValue)
            {
                hsString += "\nNew High Score!: " + value;
                PlayerPrefs.SetFloat(key, value);
            }
            else
            {
                hsString += "\nHigh Score: " + hsValue;
            }
        }
        else
        {
            PlayerPrefs.SetFloat(key, value);
        }
        text.text += (text.text.Equals("") ? "" : "\n") + hsString;
    }

    private void SetHighScore(TextMeshProUGUI text)
    {

        text.text = "";
        AddHighScore(text, highScoreKey, waveBar.WavesCompleted, "Waves Completed: ", PlayerPrefsType.INT);
        text.text += "\n";
        AddHighScore(text, enemiesKilledKey, GameManager._Instance.EnemiesKilled, "Enemies Killed: ", PlayerPrefsType.INT);

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

    private void Start()
    {
        // Reset
        // PlayerPrefs.SetFloat(highScoreKey, 0);
        // PlayerPrefs.SetFloat(enemiesKilledKey, 0);
    }
}
