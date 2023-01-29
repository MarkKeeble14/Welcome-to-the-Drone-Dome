using UnityEngine;

[System.Serializable]
public struct LoadBoolSwitchInfo
{
    private BoolSwitchUpgradeNode boolSwitch;
    public BoolSwitchUpgradeNode BoolSwitch => boolSwitch;
    [SerializeField] private int index;
    public int Index => index;

    public void SetBoolSwitch(BoolSwitchUpgradeNode boolSwitch)
    {
        this.boolSwitch = boolSwitch;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public LoadBoolSwitchInfo(BoolSwitchUpgradeNode boolSwitch, int index)
    {
        this.boolSwitch = boolSwitch;
        this.index = index;
    }
}
