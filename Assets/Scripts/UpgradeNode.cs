using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public abstract class UpgradeNode : ScriptableObject
{
    public bool Purchased;
    public string Label;
    public UpgradeNode[] Requirements;
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
    public abstract bool Maxed();

    public abstract void Purchase();
    public virtual void Reset()
    {
        Purchased = false;
    }

    public virtual void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        // Default is nothing, but this can be overridden in a child class to set extra UI, such as points
    }
}
