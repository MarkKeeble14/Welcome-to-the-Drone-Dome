using UnityEngine;

[System.Serializable]
public class DropMap
{
    public Drop resourceDrop;
    public Drop moduleDrop;
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
        foreach (GameObject module in moduleDrop.DropObjects(1))
        {
            ShopManager._Instance.NumModulesActive++;
            ModuleScavengeableParent mParent = module.GetComponent<ModuleScavengeableParent>();
            mParent.SetFromOptions(GameManager._Instance.AllModules, position);
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
