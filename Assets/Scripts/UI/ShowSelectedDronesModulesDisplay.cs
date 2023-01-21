using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowSelectedDronesModulesDisplay : MonoBehaviour
{
    [SerializeField] private SelectedDronesModuleDisplay UIPrefab;

    [SerializeField] private Transform passivesListParent;
    [SerializeField] private TextMeshProUGUI passivesLabel;
    [SerializeField] private Transform activesListParent;
    [SerializeField] private TextMeshProUGUI activesLabel;
    [SerializeField] private Transform weaponsListParent;
    [SerializeField] private TextMeshProUGUI weaponsLabel;
    [SerializeField] private Transform otherListParent;
    [SerializeField] private TextMeshProUGUI otherLabel;

    private List<SelectedDronesModuleDisplay> spawnedList = new List<SelectedDronesModuleDisplay>();
    private List<DroneModule> addedModules = new List<DroneModule>();

    [SerializeField] private PlayerDroneController playerDroneController;

    private void Update()
    {
        SetLabels();
    }

    private void SetLabels()
    {
        passivesLabel.text = "Passives: " + DroneModule.GetNumModulesOfCategory(ModuleCategory.PASSIVE, addedModules) + "/" + playerDroneController.SelectedDrone.PassivesPerDrone;
        activesLabel.text = "Actives: " + DroneModule.GetNumModulesOfCategory(ModuleCategory.ACTIVE, addedModules) + "/" + playerDroneController.SelectedDrone.ActivesPerDrone;
        weaponsLabel.text = "Weapons: " + DroneModule.GetNumModulesOfCategory(ModuleCategory.WEAPON, addedModules) + "/" + playerDroneController.SelectedDrone.WeaponsPerDrone;
    }

    public void Set(List<DroneModule> modules, bool interactable)
    {
        Clear();
        for (int i = 0; i < modules.Count; i++)
        {
            AddModule(modules[i], interactable);
        }
    }

    public void AddModule(DroneModule module, bool interactable)
    {
        addedModules.Add(module);
        SelectedDronesModuleDisplay spawned = null;
        switch (module.Category)
        {
            case ModuleCategory.ACTIVE:
                spawned = Instantiate(UIPrefab, activesListParent);
                break;
            case ModuleCategory.PASSIVE:
                spawned = Instantiate(UIPrefab, passivesListParent);
                break;
            case ModuleCategory.WEAPON:
                spawned = Instantiate(UIPrefab, weaponsListParent);
                break;
        }
        spawned.Set(module, () => UpgradeManager._Instance.ShowUpgradeTree(module.UpgradeTree), interactable);
        spawnedList.Add(spawned);
    }

    public void Clear()
    {
        foreach (SelectedDronesModuleDisplay display in spawnedList)
        {
            Destroy(display.gameObject);
        }
        spawnedList.Clear();
        addedModules.Clear();
    }
}
