using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropMap
{
    public Drop resourceDrop;
    public Drop droneModuleDrop;
    public Drop upgradeNodeDrop;
    public Drop heartDrop;

    public void DropResources(Vector3 position)
    {
        foreach (GameObject resource in resourceDrop.DropObjects(ShopManager._Instance.ResourceDropRateModifier))
        {
            resource.transform.position = position;
            resource.transform.localScale = Vector3.one;
        }
    }

    public void DropModules(Vector3 position)
    {
        foreach (GameObject module in droneModuleDrop.DropObjects(1))
        {
            ShopManager._Instance.NumModulesActive++;
            DroneModuleScavengeableParent mParent = module.GetComponent<DroneModuleScavengeableParent>();
            mParent.SetFromOptions(GameManager._Instance.AllModules, position);
            module.transform.localScale = Vector3.one;
        }
    }

    public void DropUpgradeNodes(Vector3 position)
    {
        foreach (GameObject module in upgradeNodeDrop.DropObjects(1))
        {
            DroneModuleUpgradeScavengeableParent mParent = module.GetComponent<DroneModuleUpgradeScavengeableParent>();
            mParent.SetFromOptions(UpgradeManager._Instance.AllUpgradeTrees, position);
            module.transform.localScale = Vector3.one;
        }
    }

    public void DropHearts(Vector3 position)
    {
        foreach (GameObject heart in heartDrop.DropObjects(1))
        {
            heart.transform.position = position;
            heart.transform.localScale = Vector3.one;
        }
    }
}
