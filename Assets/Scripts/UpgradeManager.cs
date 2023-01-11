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

    [SerializeField] private int upgradePointsAvailable;
    public int UpgradePointsAvailable => upgradePointsAvailable;

    private List<UpgradeTree> availableUpgradeTrees = new List<UpgradeTree>();
    [Header("References")]
    [SerializeField] private ShowSelectedDronesModulesDisplay showModulesDisplay;
    [SerializeField] private UpgradeNodeDisplay upgradeNodePrefab;
    [SerializeField] private Transform upgradeTreeDisplay;
    [SerializeField] private Transform upgradeTreeNodeParent;
    private List<UpgradeNodeDisplay> spawnedUpgradeNodes = new List<UpgradeNodeDisplay>();
    [SerializeField] private TextMeshProUGUI sectionText;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private Button backButton;
    [SerializeField] private Button doneButton;
    [SerializeField] private DronesDisplay upgradeDronesDisplay;

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

        ShowSelectedDroneModules();
        // ShowSelectedDroneUpgradeTree();
    }

    private void ShowSelectedDroneModules()
    {
        upgradeTreeDisplay.gameObject.SetActive(false);
        showModulesDisplay.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(true);
        upgradeDronesDisplay.gameObject.SetActive(true);

        showModulesDisplay.Set(playerDroneController.SelectedDrone.AppliedModules);
    }

    public void ShowModuleUpgradeTree(DroneModule module)
    {
        upgradeTreeDisplay.gameObject.SetActive(true);
        showModulesDisplay.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        doneButton.gameObject.SetActive(false);
        upgradeDronesDisplay.gameObject.SetActive(false);

        ShowUpgradeTree(module.UpgradeTree);
    }

    private void ShowUpgradeTree(UpgradeTree tree)
    {
        DestroyShownUpgradeNodes();

        // Debug.Log("Showing Tree: " + tree);
        spawnedUpgradeNodes = tree.ShowNodes(upgradeNodePrefab, upgradeTreeNodeParent);
        sectionText.text = tree.Label;
    }

    public void CloseUpgradeTree()
    {
        // Resume Game
        PauseManager._Instance.Resume();

        // UI
        UIManager._Instance.CloseUpgradeUI();
        UIManager._Instance.OpenInGameUI();
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

    public void Back()
    {
        upgradeTreeDisplay.gameObject.SetActive(false);
        showModulesDisplay.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        doneButton.gameObject.SetActive(true);
        upgradeDronesDisplay.gameObject.SetActive(true);
    }
}
