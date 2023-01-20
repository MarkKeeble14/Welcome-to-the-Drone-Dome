using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeTree
{
    public string Label;
    [SerializeField] private List<UpgradeNode> nodes = new List<UpgradeNode>();

    public bool HasNewlyUnlockedNode
    {
        get
        {
            foreach (UpgradeNode node in nodes)
            {
                if (node.NewlyUnlocked)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void ResetNewlyUnlockedNodes()
    {
        foreach (UpgradeNode node in nodes)
        {
            node.NewlyUnlocked = false;
        }
    }

    [SerializeField] private UpgradeTreeRelation upgradeTreeRelation;
    public UpgradeTreeRelation UpgradeTreeRelation => upgradeTreeRelation;

    public List<UpgradeNodeDisplay> ShowNodes(UpgradeNodeDisplay upgradeNodePrefab, Transform t, Action<UpgradeNode> onClick, bool inGame)
    {
        if (inGame)
        {
            List<UpgradeNodeDisplay> spawnedNodes = new List<UpgradeNodeDisplay>();
            for (int i = 0; i < nodes.Count; i++)
            {
                UpgradeNode node = nodes[i];
                UpgradeNodeDisplay spawned = GameObject.Instantiate(upgradeNodePrefab, t);
                spawned.Set(ref node, onClick);
                spawnedNodes.Add(spawned);
            }
            return spawnedNodes;

        }
        else
        {
            List<UpgradeNodeDisplay> spawnedNodes = new List<UpgradeNodeDisplay>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is IUpgradeNodePermanantelyUpgradeable)
                {
                    UpgradeNode node = nodes[i];
                    UpgradeNodeDisplay spawned = GameObject.Instantiate(upgradeNodePrefab, t);
                    spawned.Set(ref node, onClick);
                    spawnedNodes.Add(spawned);
                }
            }
            return spawnedNodes;
        }

    }

    public void AddNode(UpgradeNode upgradeNode)
    {
        nodes.Add(upgradeNode);
        potentialNodes.Add(upgradeNode);
    }

    private List<UpgradeNode> potentialNodes = new List<UpgradeNode>();

    public UpgradeNode GetRandomNode()
    {
        if (potentialNodes.Count <= 0) return null;

        UpgradeNode node = potentialNodes[UnityEngine.Random.Range(0, potentialNodes.Count - 1)];
        Debug.Log("Random Node: " + node);
        potentialNodes.Remove(node);

        if (node.Locked)
        {
            return node;
        }
        else
        {
            return GetRandomNode();
        }
    }
}
