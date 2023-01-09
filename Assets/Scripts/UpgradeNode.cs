using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public abstract class UpgradeNode : ScriptableObject
{
    [Header("Base Upgrade Node")]
    [SerializeField] private string label;
    public string Label => label;
    protected int currentPoints;
    public int CurrentPoints => currentPoints;
    public bool Purchased => purchased;
    protected bool purchased;
    [SerializeField] private UpgradeNode[] requirements;
    public UpgradeNode[] Requirements => requirements;
    public bool Available
    {
        get
        {
            foreach (UpgradeNode node in Requirements)
            {
                if (!node.Purchased)
                    return false;
            }
            return true;
        }
    }
    public abstract int GetMaxPoints();

    public bool Maxed()
    {
        return CurrentPoints >= GetMaxPoints();
    }

    public abstract bool Purchase();

    public virtual void Reset()
    {
        purchased = false;
    }

    public virtual void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        // Default is nothing, but this can be overridden in a child class to set extra UI, such as points
    }
}
