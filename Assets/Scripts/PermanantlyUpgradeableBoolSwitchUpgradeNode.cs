using UnityEngine;

[CreateAssetMenu(fileName = "PermanantlyUpgradeableBoolSwitchUpgradeNode", menuName = "UpgradeNode/PermanantlyUpgradeableBoolSwitchUpgradeNode")]
public class PermanantlyUpgradeableBoolSwitchUpgradeNode : BoolSwitchUpgradeNode, IUpgradeNodePermanantelyUpgradeable
{
    [Header("Permanantly Upgrade Node")]
    [SerializeField] private int numToActivate;
    private int numUpgrades;
    [SerializeField] private bool upgradeActive;
    public bool UpgradeActive => upgradeActive;


    [SerializeField] private string baseUpgradedString = "TimesPermanantUpgraded";
    private string baseUpgradedStringKey
    {
        get
        {
            return name + baseUpgradedString;
        }
    }

    public bool CanUpgradePermanantly()
    {
        return !upgradeActive;
    }

    public string GetLabel()
    {
        return Label;
    }

    public string GetToShow()
    {
        return "Reach Max to Activate: " + numUpgrades + " / " + numToActivate;
    }

    public void HardReset()
    {
        numUpgrades = 0;
        upgradeActive = false;
        SaveValue();
    }

    public void Upgrade()
    {
        if (numUpgrades >= numToActivate)
        {
            return;
        }
        numUpgrades++;
        SaveValue();
    }

    private void UpdateState()
    {
        if (numUpgrades > numToActivate)
        {
            upgradeActive = true;
        }
    }

    public void LoadValue()
    {
        if (PlayerPrefs.HasKey(baseUpgradedStringKey))
        {
            numUpgrades = PlayerPrefs.GetInt(baseUpgradedStringKey);
        }
        else
        {
            numUpgrades = 0;
        }

        UpdateState();
    }

    public void SaveValue()
    {
        PlayerPrefs.SetInt(baseUpgradedStringKey, numUpgrades);
        PlayerPrefs.Save();
    }
}
