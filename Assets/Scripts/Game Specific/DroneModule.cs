using System.Collections;
using UnityEngine;

public abstract class DroneModule : MonoBehaviour
{
    public virtual ModuleType Type { get; }
    public virtual ModuleCategory Category { get; }
    [SerializeField] private bool occupiesModuleSlot = true;
    public bool OccupiesModuleSlot => occupiesModuleSlot;

    [SerializeField] protected System.Collections.Generic.List<UpgradeNode> allModuleUpgradeNodes = new System.Collections.Generic.List<UpgradeNode>();

    [SerializeField] private UpgradeTree upgradeTree;
    public UpgradeTree UpgradeTree => upgradeTree;
    public bool HasNewlyUnlockedNode => upgradeTree.HasNewlyUnlockedNode;

    private bool attached;
    public bool Attached
    {
        get
        {
            return attached;
        }
        set
        {
            attached = value;
        }
    }

    [Header("Visual")]
    [SerializeField] private DroneModuleVisual visual;

    private void OnDestroy()
    {
        Destroy(visual.gameObject);
    }

    protected void Awake()
    {
        // Debug.Log("Module Awake: " + name);

        // Clone nodes so this module has it's own instances
        allModuleUpgradeNodes = UpgradeNode.CloneUpgradeNodesForTree(allModuleUpgradeNodes, upgradeTree);

        // Set Module Stats using new Cloned Nodes
        LoadModuleData();
    }

    protected abstract void LoadModuleData();

    public void Set(DroneController drone)
    {
        visual.Set(drone);
    }

    public static int GetNumModulesOfCategory(ModuleCategory category, System.Collections.Generic.List<DroneModule> modules)
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
