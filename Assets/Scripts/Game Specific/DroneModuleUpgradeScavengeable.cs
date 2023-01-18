using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroneModuleUpgradeScavengeable : AutoCollectScavengeable
{
    [Header("Module Scavengeable")]
    [SerializeField] private float emissionIntensity = 4f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Material material;
    [SerializeField] private ModuleScavengeableSkyPillar skyPillar;
    [SerializeField] private UpgradeNode node;
    [SerializeField] private Color color;
    [SerializeField] private string typeText;

    private void SetMaterial()
    {
        // Set Variables
        material = new Material(material);
        material.color = color;
        material.SetColor("_EmissionColor", color * emissionIntensity);
        foreach (Renderer r in renderers)
        {
            r.material = material;
        }
    }

    public override void ReleaseToPool()
    {
        ShopManager._Instance.NumModulesActive--;
        ObjectPooler._Instance.ReleaseSimpleObject(objectPoolerPrefabKey, transform.parent.gameObject);
    }

    public override void OnPickup()
    {
        skyPillar.Subside();
        node.Unlock();
        ObjectPooler.popupTextPool.Get().Set("Unlocked: " + typeText.ToString(), color, transform.position, 2);
        ReleaseToPool();
    }

    private void Cancel()
    {
        ReleaseToPool();
    }

    public void Set(List<UpgradeTree> trees)
    {
        // Get's a random node to unlock from upgrade tree
        UpgradeTree selectedTree = trees[Random.Range(0, trees.Count)];
        node = selectedTree.GetRandomNode();

        if (node == null) Cancel();

        UpgradeTreeDisplayInfo info = GameManager._Instance.GetUpgradeTreeDisplayInfo(selectedTree.UpgradeTreeRelation);
        color = info.Color;
        typeText = info.Text + "\n" + node.ShortLabel;

        SetMaterial();
        text.color = color;
        text.text = typeText.ToString();

        skyPillar.Reach();
    }
}
