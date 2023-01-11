using System.Collections.Generic;
using UnityEngine;

public abstract class DroneModule : MonoBehaviour
{
    public virtual ModuleType Type { get; }
    public virtual ModuleCategory Category { get; }
    [SerializeField] private bool occupiesModuleSlot = true;
    public bool OccupiesModuleSlot => occupiesModuleSlot;

    [SerializeField] protected List<UpgradeNode> allModuleUpgradeNodes = new List<UpgradeNode>();

    [SerializeField] private UpgradeTree upgradeTree;
    public UpgradeTree UpgradeTree => upgradeTree;

    protected void Awake()
    {
        // Debug.Log("Module Awake: " + name);

        // Clone nodes so this module has it's own instances
        allModuleUpgradeNodes = UpgradeNode.CloneUpgradeNodesForTree(allModuleUpgradeNodes, upgradeTree);

        // Set Module Stats using new Cloned Nodes
        LoadModuleData();
    }

    protected abstract void LoadModuleData();

    public static int GetNumModulesOfCategory(ModuleCategory category, List<DroneModule> modules)
    {
        int x = 0;
        foreach (DroneModule module in modules)
        {
            if (module.Category == category && module.OccupiesModuleSlot)
                x++;
        }
        return x;
    }
}
