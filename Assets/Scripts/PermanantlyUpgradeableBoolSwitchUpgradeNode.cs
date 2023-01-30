using UnityEngine;

[CreateAssetMenu(fileName = "PermanantlyUpgradeableBoolSwitchUpgradeNode", menuName = "UpgradeNode/PermanantlyUpgradeableBoolSwitchUpgradeNode")]
public class PermanantlyUpgradeableBoolSwitchUpgradeNode : BoolSwitchUpgradeNode, IUpgradeNodePermanantelyUpgradeable
{
    [Header("Permanantly Upgrade Node")]
    [SerializeField] private int numToActivate;
    private int numUpgrades;
    [SerializeField] private bool upgradeActive;
    public bool UpgradeActive => upgradeActive;

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
    }

    public void Upgrade()
    {
        if (numUpgrades >= numToActivate)
        {
            return;
        }
        numUpgrades++;
        if (numUpgrades > numToActivate)
        {
            upgradeActive = true;
        }
    }
}
