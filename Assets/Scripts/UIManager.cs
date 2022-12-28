using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }

    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject inGameUI;

    public void OpenInGameUI()
    {
        inGameUI.SetActive(true);
        shopUI.SetActive(false);
        upgradeUI.SetActive(false);
    }

    public void OpenShopUI()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(true);
        upgradeUI.SetActive(false);
    }

    public void OpenUpgradeUI()
    {
        inGameUI.SetActive(false);
        shopUI.SetActive(false);
        upgradeUI.SetActive(true);
    }

    public void CloseInGameUI()
    {
        inGameUI.SetActive(false);
    }

    public void CloseShopUI()
    {
        shopUI.SetActive(false);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
    }
}
