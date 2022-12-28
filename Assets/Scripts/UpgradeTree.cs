using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeTree
{
    [SerializeField] private List<UpgradeNode> nodes;

    public void ResetNodes()
    {
        foreach (UpgradeNode node in nodes)
        {
            node.Reset();
        }
    }

    public List<UpgradeNodeDisplay> ShowNodes(UpgradeNodeDisplay upgradeNodePrefab, Transform t)
    {
        List<UpgradeNodeDisplay> spawnedNodes = new List<UpgradeNodeDisplay>();
        foreach (UpgradeNode node in nodes)
        {
            UpgradeNodeDisplay spawned = GameObject.Instantiate(upgradeNodePrefab, t);
            spawnedNodes.Add(spawned);
            spawned.Set(node, () => UpgradeManager._Instance.TryPurchaseNode(node));
        }
        return spawnedNodes;
    }
}
