using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager _Instance;

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }

    private int pointsAvailable;
    public int PointsAvailable => pointsAvailable;

    [SerializeField] private List<UpgradeTree> defaultUpgradeTrees = new List<UpgradeTree>();
    private List<UpgradeTree> availableUpgradeTrees = new List<UpgradeTree>();
    private int upgradeTreeIndex;
    [SerializeField]
    private SerializableDictionary<ModuleType, UpgradeTree> weaponModuleUpgradeTrees
        = new SerializableDictionary<ModuleType, UpgradeTree>();

    [SerializeField] private UpgradeNodeDisplay upgradeNodePrefab;
    [SerializeField] private Transform upgradeTreeNodeParent;
    private List<UpgradeNodeDisplay> spawnedUpgradeNodes = new List<UpgradeNodeDisplay>();
    [SerializeField] private TextMeshProUGUI sectionText;

    private void Start()
    {
        ResetAllNodes();
    }

    public void AddUpgradePoints(int amount)
    {
        pointsAvailable += amount;
    }

    private void ResetAllNodes()
    {
        List<UpgradeTree> upgradeTrees = weaponModuleUpgradeTrees.Values();
        foreach (UpgradeTree tree in upgradeTrees)
        {
            tree.ResetNodes();
        }
    }

    private void DestroyShownUpgradeNodes()
    {
        // Remove all old offering game objects/displays
        foreach (Transform child in upgradeTreeNodeParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void OpenUpgradeTree()
    {
        // Pause Game
        PauseManager._Instance.Pause();

        // UI
        UIManager._Instance.OpenUpgradeUI();

        SetAvailableUpgradeTrees();

        // Show Upgrade Tree
        if (availableUpgradeTrees.Count > 0)
            ShowUpgradeTree(availableUpgradeTrees[upgradeTreeIndex]);
    }

    private void SetAvailableUpgradeTrees()
    {
        // Clear the previously set list of upgrade trees
        availableUpgradeTrees.Clear();

        // Add the upgrade trees that should always be available
        availableUpgradeTrees.AddRange(defaultUpgradeTrees);

        // For each weapon type that the player has equipped, we add that weapon types upgrade tree to the list of available
        // upgrade trees
        foreach (ModuleType type in GameManager._Instance.DroneWeaponModules)
        {
            // Get the entry 
            SerializableKeyValuePair<ModuleType, UpgradeTree> kvp = weaponModuleUpgradeTrees.GetEntry(type);

            // If it doesn't exist, there are no upgrades available for that weapon type
            if (kvp == null) continue;

            // Otherwise, if the upgrade tree has not already been added, add it to the list of upgrade trees
            UpgradeTree tree = kvp.Value;
            if (!availableUpgradeTrees.Contains(tree))
            {
                availableUpgradeTrees.Add(tree);
            }
        }
    }

    private void UpdateUpgradeTree()
    {
        if (availableUpgradeTrees.Count <= 0) return;

        // 
        UpgradeTree tree = availableUpgradeTrees[upgradeTreeIndex];

        // 
        ShowUpgradeTree(tree);
    }

    public void CloseUpgradeTree()
    {
        // Resume Game
        PauseManager._Instance.Resume();

        // UI
        UIManager._Instance.CloseUpgradeUI();
        UIManager._Instance.OpenInGameUI();
    }

    private void ShowUpgradeTree(UpgradeTree tree)
    {
        DestroyShownUpgradeNodes();

        // Debug.Log("Showing Tree: " + tree);
        spawnedUpgradeNodes = tree.ShowNodes(upgradeNodePrefab, upgradeTreeNodeParent);
        sectionText.text = tree.Label;
    }

    public void TryPurchaseNode(UpgradeNode node)
    {
        if (!node.Available)
        {
            Debug.Log("Failed to Purchase: " + node.Label + ", Node Unavailable");
            return;
        }

        if (pointsAvailable <= 0)
        {
            Debug.Log("Failed to Purchase: " + node.Label + ", No Points Available");
            return;
        }

        if (node.Maxed())
        {
            Debug.Log("Failed to Purchase: " + node.Label + ", Already Maxed Out");
            return;
        }

        node.Purchase();
        pointsAvailable--;
        UpdateUpgradeTree();
        Debug.Log("Successfully Purchased: " + node.Label);
    }

    public void LastUpgradeTree()
    {
        if (upgradeTreeIndex > 0)
        {
            upgradeTreeIndex--;
            UpdateUpgradeTree();
        }
        else if (upgradeTreeIndex == 0)
        {
            upgradeTreeIndex = availableUpgradeTrees.Count - 1;
            UpdateUpgradeTree();
        }
    }

    public void NextUpgradeTree()
    {
        if (upgradeTreeIndex < availableUpgradeTrees.Count - 1)
        {
            upgradeTreeIndex++;
            UpdateUpgradeTree();
        }
        else if (upgradeTreeIndex == availableUpgradeTrees.Count - 1)
        {
            upgradeTreeIndex = 0;
            UpdateUpgradeTree();
        }
    }
}
