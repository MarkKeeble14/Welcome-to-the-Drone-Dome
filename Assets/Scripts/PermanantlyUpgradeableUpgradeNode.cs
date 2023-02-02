using UnityEngine;

public interface IUpgradeNodePermanantelyUpgradeable
{
    public void HardReset();
    public void Upgrade();
    public string GetLabel();
    public string GetToShow();
    public bool CanUpgradePermanantly();
    public void LoadValue();
    public void SaveValue();
}