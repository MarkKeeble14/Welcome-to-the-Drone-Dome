using System;
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
    public List<UpgradeTree> AllAvailableUpgradeTrees
    {
        get
        {
            List<UpgradeTree> trees = new List<UpgradeTree>();
            foreach (DroneController drone in playerDroneController.TrackedDrones)
            {
                foreach (DroneModule module in drone.AppliedModules)
                {
                    trees.Add(module.UpgradeTree);
                }
            }
            trees.AddRange(otherUpgradeTrees);
            return trees;
        }
    }

    [Header("References")]
    [SerializeField] private ShowSelectedDronesModulesDisplay showModulesDisplay;
    [SerializeField] private ShowUpgradeTreeOptions upgradeTreeOptionsDisplay;
    [SerializeField] private Transform upgradeTreeDisplay;
    [SerializeField] private GameObject StatSheet;
    [SerializeField] private UpgradeNodeDisplay inGameUpgradeNodePrefab;
    [SerializeField] private StatSheetNode statSheetNodePrefab;
    [SerializeField] private Transform statSheetNodeList;
    [SerializeField] private Transform upgradeTreeNodeParent;
    private List<UpgradeNodeDisplay> spawnedUpgradeNodes = new List<UpgradeNodeDisplay>();
    private List<StatSheetNode> spawnedStatSheetNodes = new List<StatSheetNode>();

    [SerializeField] private TextMeshProUGUI sectionText;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private Button backButton;
    [SerializeField] private Button doneButton;

    private UpgradeUIState CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;
    private UpgradeUIState PreviousUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip openUpgradeScreenClip;
    [SerializeField] private AudioClip purchaseUpgradeClip;
    [SerializeField] private AudioClip failPurchaseUpgradeClip;
    [SerializeField] private AudioClip clickClip;

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

    private void DestroyStatSheetNodes()
    {
        // Remove all old offering game objects/displays
        foreach (Transform child in statSheetNodeList)
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

        sfxSource.PlayOneShot(openUpgradeScreenClip);
    }

    public void CloseUpgradeTree()
    {
        foreach (UpgradeTree tree in AllAvailableUpgradeTrees)
        {
            tree.ResetNewlyUnlockedNodes();
        }

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
        StatSheet.SetActive(false);
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
                StatSheet.SetActive(true);
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
            drone =>
            {
                ShowDroneModules(drone);

                // Audio
                sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
                sfxSource.PlayOneShot(clickClip);
                sfxSource.pitch = 1f;
            },
            tree =>
            {
                ShowUpgradeTree(tree);

                // Audio
                sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
                sfxSource.PlayOneShot(clickClip);
                sfxSource.pitch = 1f;
            }
            );
    }

    private void ShowDroneModules(DroneController drone)
    {
        PreviousUIState = CurrentUIState;
        CurrentUIState = UpgradeUIState.SHOW_DRONE_MODULES;
        ShowUI(CurrentUIState);

        // Debug.Log("Show Selected Drone Modules: " + drone);
        showModulesDisplay.Set(drone.AppliedModules, true);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(clickClip);
        sfxSource.pitch = 1f;
    }

    public void ShowUpgradeTree(UpgradeTree tree)
    {
        PreviousUIState = CurrentUIState;
        CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE;
        ShowUI(CurrentUIState);

        DestroyStatSheetNodes();
        DestroyShownUpgradeNodes();

        // Debug.Log("Showing Tree: " + tree);
        spawnedUpgradeNodes = tree.ShowNodes(inGameUpgradeNodePrefab, upgradeTreeNodeParent, node =>
        {
            TryPurchaseNode(node);

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(clickClip);
            sfxSource.pitch = 1f;
        }, true);
        foreach (UpgradeNodeDisplay upgradeNodeDisplay in spawnedUpgradeNodes)
        {
            StatSheetNode spawned = Instantiate(statSheetNodePrefab, statSheetNodeList);
            spawned.Set(upgradeNodeDisplay.GetNode());
        }
        sectionText.text = tree.Label;

        // Audio
        sfxSource.PlayOneShot(clickClip);
    }

    public void Back()
    {
        if (CurrentUIState == UpgradeUIState.SHOW_DRONE_MODULES)
        {
            CurrentUIState = UpgradeUIState.SHOW_UPGRADE_TREE_OPTIONS;
            ShowUI(CurrentUIState);

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(clickClip);
            sfxSource.pitch = 1f;
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

            // Audio
            sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
            sfxSource.PlayOneShot(clickClip);
            sfxSource.pitch = 1f;
        }
    }

    public void TryPurchaseNode(UpgradeNode node)
    {
        if (!node.Available)
        {
            // Debug.Log("Failed to Purchase: " + node.Label + ", Node Unavailable");

            // Audio
            sfxSource.PlayOneShot(failPurchaseUpgradeClip);

            return;
        }

        if (upgradePointsAvailable <= 0)
        {
            // Debug.Log("Failed to Purchase: " + node.Label + ", No Points Available");

            // Audio
            sfxSource.PlayOneShot(failPurchaseUpgradeClip);

            return;
        }

        if (!node.Purchase())
        {
            // Debug.Log("Failed to Purchase: " + node.Label + ", Already Maxed Out");

            // Audio
            sfxSource.PlayOneShot(failPurchaseUpgradeClip);

            return;
        }

        // Audio

        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(purchaseUpgradeClip);
        sfxSource.pitch = 1f;

        upgradePointsAvailable--;

        // UpdateUpgradeTree();
        Debug.Log("Successfully Purchased: " + node.Label);
    }
}
