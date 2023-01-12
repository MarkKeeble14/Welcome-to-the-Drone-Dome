using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeTree
{
    public string Label;
    [SerializeField] private List<UpgradeNode> nodes = new List<UpgradeNode>();

    public List<UpgradeNodeDisplay> ShowNodes(UpgradeNodeDisplay upgradeNodePrefab, Transform t)
    {
        List<UpgradeNodeDisplay> spawnedNodes = new List<UpgradeNodeDisplay>();
        for (int i = 0; i < nodes.Count; i++)
        {
            UpgradeNode node = nodes[i];
            UpgradeNodeDisplay spawned = GameObject.Instantiate(upgradeNodePrefab, t);
            spawned.Set(ref node);
            spawnedNodes.Add(spawned);

        }
        return spawnedNodes;
    }

    public void AddNode(UpgradeNode upgradeNode)
    {
        nodes.Add(upgradeNode);
    }
}
