using System.Collections.Generic;
using UnityEngine;

public class PermanantUpgradeShopManager : MonoBehaviour
{
    public static PermanantUpgradeShopManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [SerializeField] private List<UpgradeTree> allImplementedUpgradeTrees = new List<UpgradeTree>();

    [Header("References")]
    [SerializeField] private GameObject treesDisplay;
    [SerializeField] private Transform treeDisplayList;
    [SerializeField] private GameObject nodesDisplay;
    [SerializeField] private Transform nodeDisplayList;
    [SerializeField] private UpgradeTreeDisplay upgradeTreeDisplayPrefab;
    [SerializeField] private PermaUpgradeNodeDisplay permaUpgradeNodePrefab;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject exitButton;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip openClip;

    [SerializeField] private StoreInt playerCredits;

    private List<UpgradeNodeDisplay> spawnedUpgradeNodes = new List<UpgradeNodeDisplay>();

    private bool hasSpawnedTrees = false;

    public void Open()
    {
        // Audio
        AudioManager._Instance.PlayClip(openClip, true);

        // Resume Game
        PauseManager._Instance.Pause(PauseCondition.OPEN_CREDIT_SHOP);

        MainMenuUIManager._Instance.OpenPermanantUpgradeShop();

        if (hasSpawnedTrees) return;
        hasSpawnedTrees = true;
        foreach (UpgradeTree tree in allImplementedUpgradeTrees)
        {
            UpgradeTreeDisplay spawned = Instantiate(upgradeTreeDisplayPrefab, treeDisplayList);
            spawned.Set(tree, () =>
            {
                spawnedUpgradeNodes = ShowUpgradeTree(tree);
            });
        }
    }

    private void DestroyPastTree()
    {
        foreach (UpgradeNodeDisplay nodeDisplay in spawnedUpgradeNodes)
        {
            Destroy(nodeDisplay.gameObject);
        }
        spawnedUpgradeNodes.Clear();
    }

    private List<UpgradeNodeDisplay> ShowUpgradeTree(UpgradeTree tree)
    {
        DestroyPastTree();

        treesDisplay.SetActive(false);
        nodesDisplay.SetActive(true);

        backButton.SetActive(true);
        exitButton.SetActive(false);

        return tree.ShowNodes(permaUpgradeNodePrefab, nodeDisplayList, node => TryPermaGrowNode(node), false);
    }

    public void TryPermaGrowNode(UpgradeNode node)
    {
        Debug.Log("Attempting to Permanantly Upgrade: " + node);
        if (playerCredits.Value > 0)
        {
            playerCredits.Value--;
            ((IUpgradeNodePermanantelyUpgradeable)node).Upgrade();
            Debug.Log("Purchased: " + node);
        }
        else
        {
            Debug.Log("Insufficient Credits");
        }
    }

    public void HideUpgradeTree()
    {
        // Audio
        AudioManager._Instance.PlayClip(clickClip, true);

        treesDisplay.SetActive(true);
        nodesDisplay.SetActive(false);
        backButton.SetActive(false);
        exitButton.SetActive(true);
    }

    public void Close()
    {
        // Audio
        AudioManager._Instance.PlayClip(clickClip, true);

        // Resume Game
        PauseManager._Instance.Resume(PauseCondition.OPEN_CREDIT_SHOP);

        MainMenuUIManager._Instance.ClosePermanantUpgradeShop();
    }
}
