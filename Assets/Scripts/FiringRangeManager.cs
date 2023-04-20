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
    [SerializeField] private Transform listParent;
    [SerializeField] private FiringRangeModuleButton firingRangeModuleButton;
    [SerializeField] private PlayerDroneController playerDroneController;
    [SerializeField] private TextMeshProUGUI firingRangeHelperText;

    [Header("Audio")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip openClip;

    private void Start()
    {
        GenerateModuleList();
    }

    public void Open()
    {
        // Audio
        AudioManager._Instance.PlayClip(openClip, true);
        MainMenuUIManager._Instance.OpenFiringRange();

        PauseManager._Instance.Pause(PauseCondition.OPEN_FIRING_RANGE);

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
            spawned.Set(type, playerDroneController, () =>
            {
                if (playerDroneController.SelectedDrone == null)
                {
                    spawned.SetPurchased(false);
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
        // Audio
        AudioManager._Instance.PlayClip(clickClip, true);

        MainMenuUIManager._Instance.CloseFiringRange();
        PauseManager._Instance.Resume(PauseCondition.OPEN_FIRING_RANGE);

        UIManager._Instance.SetCurrentDronesDisplayForMenu();
        UIManager._Instance.CurrentDroneDisplay.Set();
    }
}
