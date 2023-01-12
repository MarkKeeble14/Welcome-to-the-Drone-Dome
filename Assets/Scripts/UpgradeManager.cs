using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [Header("Currency")]
    [SerializeField] private int upgradePointsAvailable;
    public int UpgradePointsAvailable => upgradePointsAvailable;

    [Header("Upgradeable")]
    [SerializeField] private UpgradeTree playerMovementUpgradeTree;
    public UpgradeTree PlayerMovementUpgradeTree => playerMovementUpgradeTree;
    private List<UpgradeTree> otherUpgradeTrees = new List<UpgradeTree>();

    [Header("References")]
    [SerializeField] private ShowSelectedDronesModulesDisplay showModulesDisplay;
    [SerializeField] private ShowUpgradeTreeOptions upgradeTreeOptionsDisplay;
    [SerializeField] private UpgradeNodeDisplay upgradeNodePrefab;
    [SerializeField] private Transform upgradeTreeDisplay;
    [SerializeField] private Transform upgradeTreeNodeParent;
    private List<UpgradeNodeDisplay> spawnedUpgradeNodes = new List<UpgradeNodeDisplay>();
    [SerializeField] private TextMeshProUGUI sectionText;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private Button backButton;
    [SerializeField] private Button doneButton;

    private UpgradeUIState CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;
    private UpgradeUIState PreviousUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;

    private void Start()
    {
        otherUpgradeTrees.Add(playerMovementUpgradeTree);
    }

    public void AddUpgradePoints(int amount)
    {
        upgradePointsAvailable += amount;
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

        ShowUpgradeTreeOptions();
    }

    public void CloseUpgradeTree()
    {
        // Resume Game
        PauseManager._Instance.Resume();

        // UI
        UIManager._Instance.CloseUpgradeUI();
        UIManager._Instance.OpenInGameUI();
    }

    private void ShowUI(UpgradeUIState state)
    {
        upgradeTreeOptionsDisplay.gameObject.SetActive(false);
        upgradeTreeDisplay.gameObject.SetActive(false);
        showModulesDisplay.gameObject.SetActive(false);
        switch (state)
        {
            case UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS:
                upgradeTreeOptionsDisplay.gameObject.SetActive(true);
                // Buttons
                backButton.gameObject.SetActive(false);
                doneButton.gameObject.SetActive(true);
                break;
            case UpgradeUIState.SHOW_DRONE_MODULES:
                showModulesDisplay.gameObject.SetActive(true);
                // Buttons
                backButton.gameObject.SetActive(true);
                doneButton.gameObject.SetActive(false);
                break;
            case UpgradeUIState.SHOW_UPGRADE_TREE:
                upgradeTreeDisplay.gameObject.SetActive(true);
                // Buttons
                backButton.gameObject.SetActive(true);
                doneButton.gameObject.SetActive(false);
                break;
        }
    }

    private void ShowUpgradeTreeOptions()
    {
        PreviousUIState = CurrentUIState;
        CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;
        ShowUI(CurrentUIState);

        upgradeTreeOptionsDisplay.Set(
            playerDroneController.TrackedDrones,
            otherUpgradeTrees,
            drone => ShowDroneModules(drone),
            tree => ShowUpgradeTree(tree)
            );
    }

    private void ShowDroneModules(DroneController drone)
    {
        PreviousUIState = CurrentUIState;
        CurrentUIState = UpgradeUIState.SHOW_DRONE_MODULES;
        ShowUI(CurrentUIState);

        // Debug.Log("Show Selected Drone Modules: " + drone);
        showModulesDisplay.Set(drone.AppliedModules);
    }

    public void ShowUpgradeTree(UpgradeTree tree)
    {
        PreviousUIState = CurrentUIState;
        CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE;
        ShowUI(CurrentUIState);

        DestroyShownUpgradeNodes();

        // Debug.Log("Showing Tree: " + tree);
        spawnedUpgradeNodes = tree.ShowNodes(upgradeNodePrefab, upgradeTreeNodeParent);
        sectionText.text = tree.Label;
    }

    public void Back()
    {
        if (CurrentUIState == UpgradeUIState.SHOW_DRONE_MODULES)
        {
            CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;
            ShowUI(CurrentUIState);
        }
        else if (CurrentUIState == UpgradeUIState.SHOW_UPGRADE_TREE)
        {
            if (PreviousUIState == UpgradeUIState.SHOW_DRONE_MODULES)
            {
                CurrentUIState = UpgradeUIState.SHOW_DRONE_MODULES;
            }
            else
            {
                CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;
            }
            ShowUI(CurrentUIState);
        }
    }

    public void TryPurchaseNode(UpgradeNode node)
    {
        if (!node.Available)
        {
            Debug.Log("Failed to Purchase: " + node.Label + ", Node Unavailable");
            return;
        }

        if (upgradePointsAvailable <= 0)
        {
            Debug.Log("Failed to Purchase: " + node.Label + ", No Points Available");
            return;
        }

        if (!node.Purchase())
        {
            Debug.Log("Failed to Purchase: " + node.Label + ", Already Maxed Out");
            return;
        }

        upgradePointsAvailable--;
        // UpdateUpgradeTree();
        Debug.Log("Successfully Purchased: " + node.Label);
    }
}
