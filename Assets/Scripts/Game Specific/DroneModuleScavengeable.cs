using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroneModuleScavengeable : AutoCollectScavengeable
{
    [Header("Module Scavengeable")]
    [SerializeField] private float emissionIntensity = 4f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Material material;
    [SerializeField] private ModuleScavengeableSkyPillar skyPillar;
    private ModuleType type;
    private Color color;

    protected override void OnEnable()
    {
        // Tell Object what to Release
        SetReleaseable(transform.parent.gameObject);
    }

    public void Set(List<ModuleType> possibleWeaponModules)
    {
        if (!ShopManager._Instance.AllowModuleDrops)
        {
            Cancel();
            return;
        }

        type = RandomHelper.GetRandomFromList(possibleWeaponModules);
        color = GameManager._Instance.GetModuleColor(type);

        SetMaterial();
        text.color = color;
        text.text = type.ToString();

        skyPillar.Reach();
    }

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
        base.ReleaseToPool();
    }

    public override void PickupScavengeable()
    {
        skyPillar.Subside();
        ShopManager._Instance.PickedUpModule(type);

        PopupText text = ObjectPooler.popupTextPool.Get();
        text.Set("+" + type.ToString(), color, transform.position, 2);

        base.PickupScavengeable();
    }

    private void Cancel()
    {
        ReleaseToPool();
    }
}
