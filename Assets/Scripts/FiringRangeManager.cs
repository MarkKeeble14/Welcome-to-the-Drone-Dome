using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FiringRangeManager : MonoBehaviour
{
    public static FiringRangeManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
    }

    private Dictionary<ModuleType, FiringRangeModuleButton> spawnedModuleButtons = new Dictionary<ModuleType, FiringRangeModuleButton>();

    [Header("References")]
    [SerializeField] private GameObject regularUI;
    [SerializeField] private GameObject firingRangeUI;
    [SerializeField] private Transform listParent;
    [SerializeField] private FiringRangeModuleButton firingRangeModuleButton;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private TextMeshProUGUI firingRangeHelperText;

    private void Start()
    {
        GenerateModuleList();
    }

    public void Open()
    {
        firingRangeUI.SetActive(true);
        regularUI.SetActive(false);
        PauseManager._Instance.Pause();
        UIManager._Instance.SetCurrentDronesDisplayForFiringRange();
        UIManager._Instance.CurrentDroneDisplay.Set();
    }

    public void ResetFiringRange()
    {
        foreach (KeyValuePair<ModuleType, FiringRangeModuleButton> kvp in spawnedModuleButtons)
        {
            Destroy(kvp.Value.gameObject);
        }
        spawnedModuleButtons.Clear();
        GameManager._Instance.ResetPlayerDrones();
        GenerateModuleList();
    }

    private void GenerateModuleList()
    {
        List<ModuleType> modules = GameManager._Instance.AllModules;
        foreach (ModuleType type in modules)
        {
            FiringRangeModuleButton spawned = Instantiate(firingRangeModuleButton, listParent);
            spawnedModuleButtons.Add(type, spawned);
            DroneModule module = null;
            spawned.Set(type, () =>
            {
                if (playerDroneController.SelectedDrone == null)
                {
                    firingRangeHelperText.gameObject.SetActive(true);
                    firingRangeHelperText.text = "You Must First Select a Drone to Equip a Module";
                    return;
                }
                firingRangeHelperText.gameObject.SetActive(false);
                spawned.SetPurchased(true);
                module = GameManager._Instance.AddModule(playerDroneController.SelectedDrone, type);
            }, () =>
            {
                if (playerDroneController.SelectedDrone == null)
                {
                    firingRangeHelperText.gameObject.SetActive(true);
                    firingRangeHelperText.text = "You Must First Select a Drone to Remove a Module";
                    return;
                }
                if (!GameManager._Instance.RemoveModule(playerDroneController.SelectedDrone, module))
                {
                    firingRangeHelperText.gameObject.SetActive(true);
                    firingRangeHelperText.text = "Unable to Remove Module";
                }
                else
                {
                    spawned.SetPurchased(false);
                }
            });
        }
    }


    public void Close()
    {
        regularUI.SetActive(true);
        firingRangeUI.SetActive(false);
        PauseManager._Instance.Resume();

        UIManager._Instance.SetCurrentDronesDisplayForMenu();
        UIManager._Instance.CurrentDroneDisplay.Set();
    }
}
