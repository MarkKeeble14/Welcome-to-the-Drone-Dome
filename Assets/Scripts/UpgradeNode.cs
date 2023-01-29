using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
    public bool NewlyUnlocked { get; set; }

    public bool Available
    {
        get
        {
            foreach (UpgradeNode node in Requirements)
            {
                if (!node.Maxed())
                    return false;
            }
            return true;
        }
    }
    private bool locked = true;
    public bool Locked => locked;

    [SerializeField] private string shortLabel;
    public string ShortLabel => shortLabel;
    public abstract string GetStatState();

    public abstract int GetMaxPoints();

    public virtual bool Maxed()
    {
        return CurrentPoints >= GetMaxPoints();
    }

    public abstract bool Purchase();

    public virtual void Reset()
    {
        purchased = false;
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
        NewlyUnlocked = true;
    }

    public virtual void SetExtraUI(UpgradeNodeDisplay nodeDisplay)
    {
        // Default is nothing, but this can be overridden in a child class to set extra UI, such as points
    }

    public void UpdateRequirement(UpgradeNode newNode, int i)
    {
        // Debug.Log("Update Requirement at: " + i + ", New Node: " + newNode);
        requirements[i] = newNode;
    }

    public static StatModifierUpgradeNode GetStatModifierUpgradeNode(LoadStatModifierInfo info, List<UpgradeNode> list)
    {
        return ((StatModifierUpgradeNode)list[info.Index]);
    }

    public static BoolSwitchUpgradeNode GetBoolSwitchUpgradeNode(LoadBoolSwitchInfo info, List<UpgradeNode> list)
    {
        return (BoolSwitchUpgradeNode)list[info.Index];
    }

    public static List<UpgradeNode> CloneUpgradeNodesForTree(List<UpgradeNode> original, UpgradeTree tree)
    {
        // Update References of Nodes
        List<UpgradeNode> newNodes = new List<UpgradeNode>();
        foreach (UpgradeNode node in original)
        {
            UpgradeNode newNode = Instantiate(node);
            // Debug.Log("Cloned Node ID: " + newNode.GetInstanceID());
            newNodes.Add(newNode);
            tree.AddNode(newNode);

            /*
            if (newNode is StatModifierUpgradeNode)
                Debug.Log("Clone Stat: " + newNode.label + " - " + ((StatModifierUpgradeNode)newNode).Stat.GetInstanceID());
            */
        }

        // Update Requirements in Nodes
        foreach (UpgradeNode newNode in newNodes)
        {
            if (newNode.Requirements.Length <= 0) continue;
            for (int i = 0; i < newNode.Requirements.Length; i++)
            {
                UpgradeNode originalRequirement = newNode.Requirements[i];
                foreach (UpgradeNode checkNode in newNodes)
                {
                    // Debug.Log(originalRequirement.name + " -> " + checkNode.name.Substring(0, checkNode.name.Length - "(Clone)".Length));
                    if (originalRequirement.name == checkNode.name.Substring(0, checkNode.name.Length - "(Clone)".Length))
                    {
                        newNode.UpdateRequirement(checkNode, i);
                        break;
                    }
                }
            }
        }

        // Update List so modules can find stats here
        return newNodes;
    }
}
