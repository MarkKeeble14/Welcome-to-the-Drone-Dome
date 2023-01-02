using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowSelectedDronesModulesDisplay : MonoBehaviour
{
    [SerializeField] private SelectedDronesModuleDisplay UIPrefab;
    private List<SelectedDronesModuleDisplay> selectedDroneModulesDisplay = new List<SelectedDronesModuleDisplay>();

    [SerializeField] private Transform passivesList;
    [SerializeField] private TextMeshProUGUI passivesLabel;
    private int numPassives;
    private int numActives;
    private int numWeapons;

    [SerializeField] private Transform activesList;
    [SerializeField] private TextMeshProUGUI activesLabel;
    [SerializeField] private Transform weaponsList;
    [SerializeField] private TextMeshProUGUI weaponsLabel;

    private void SetLabels()
    {
        passivesLabel.text = "Passives: " + numPassives + "/" + GameManager._Instance.PassivesPerDrone;
        activesLabel.text = "Actives: " + numActives + "/" + GameManager._Instance.ActivesPerDrone;
        weaponsLabel.text = "Weapons: " + numWeapons + "/" + GameManager._Instance.WeaponsPerDrone;
    }

    public void Set(DroneController drone)
    {
        Clear();
        foreach (DroneModule module in drone.AppliedModules)
        {
            AddModule(module.Type);
        }
    }

    public void AddModule(ModuleType type)
    {
        switch (GameManager._Instance.GetModuleCategory(type))
        {
            case ModuleCategory.ACTIVE:
                AddActive(type);
                break;
            case ModuleCategory.PASSIVE:
                AddPassive(type);
                break;
            case ModuleCategory.WEAPON:
                AddWeapon(type);
                break;
        }
        SetLabels();
    }

    public void Clear()
    {
        // Remove Set Displays
        ClearChildrenOfTransform(passivesList);
        ClearChildrenOfTransform(activesList);
        ClearChildrenOfTransform(weaponsList);

        // Reset Labels
        numPassives = 0;
        numActives = 0;
        numWeapons = 0;
        SetLabels();
    }

    public void ClearChildrenOfTransform(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddPassive(ModuleType type)
    {
        SelectedDronesModuleDisplay spawned = Instantiate(UIPrefab, passivesList);
        spawned.Set(type, ModuleCategory.PASSIVE);
        selectedDroneModulesDisplay.Add(spawned);
        numPassives++;
    }

    private void AddActive(ModuleType type)
    {
        SelectedDronesModuleDisplay spawned = Instantiate(UIPrefab, activesList);
        spawned.Set(type, ModuleCategory.ACTIVE);
        selectedDroneModulesDisplay.Add(spawned);
        numActives++;
    }

    private void AddWeapon(ModuleType type)
    {
        SelectedDronesModuleDisplay spawned = Instantiate(UIPrefab, weaponsList);
        spawned.Set(type, ModuleCategory.WEAPON);
        selectedDroneModulesDisplay.Add(spawned);
        numWeapons++;
    }
}
