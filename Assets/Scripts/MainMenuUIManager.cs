using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    [SerializeField] private GameObject regularUI;
    [SerializeField] private GameObject firingRangeUI;
    [SerializeField] private GameObject permanantUpgradeShop;

    public void OpenFiringRange()
    {
        firingRangeUI.SetActive(true);
        regularUI.SetActive(false);

        InputManager.DisablePlayerControls();
    }

    public void CloseFiringRange()
    {
        firingRangeUI.SetActive(false);
        regularUI.SetActive(true);

        InputManager.EnablePlayerControls();
    }

    public void OpenPermanantUpgradeShop()
    {
        permanantUpgradeShop.SetActive(true);
        regularUI.SetActive(false);

        InputManager.DisablePlayerControls();
    }

    public void ClosePermanantUpgradeShop()
    {
        permanantUpgradeShop.SetActive(false);
        regularUI.SetActive(true);

        InputManager.EnablePlayerControls();
    }
}
