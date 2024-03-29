﻿using UnityEngine;

[System.Serializable]
public struct LoadStatModifierInfo
{
    private StatModifierUpgradeNode upgradeNode;
    public StatModifierUpgradeNode UpgradeNode => upgradeNode;

    public StatModifier Stat => upgradeNode.Stat;

    [SerializeField] private int index;
    public int Index => index;

    public void SetStat(StatModifierUpgradeNode upgradeNode)
    {
        this.upgradeNode = upgradeNode;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public LoadStatModifierInfo(StatModifierUpgradeNode upgradeNode, int index)
    {
        this.upgradeNode = upgradeNode;
        this.index = index;
    }
}
