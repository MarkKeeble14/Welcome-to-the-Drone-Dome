using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModuleScavengeable : Scavengeable
{
    private ModuleType type;
    [SerializeField] private float emissionIntensity = 6f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Material material;
    [SerializeField] private ModuleScavengeableSkyPillar skyPillar;
    [SerializeField] private PopupText popupText;
    private Color color;

    private void SetMaterial()
    {
        // Set Variables
        material = new Material(material);
        color = GameManager._Instance.GetModuleColor(type);
        material.color = color;
        material.SetColor("_EmissionColor", color * emissionIntensity);
        foreach (Renderer r in renderers)
        {
            r.material = material;
        }
    }

    public override void OnPickup()
    {
        ShopManager._Instance.PickedUpModule(type);
        skyPillar.Subside(true);
        Instantiate(popupText, transform.position, Quaternion.identity).Set("+" + type.ToString(),
            color, transform.position.y, 2);
        Destroy(transform.root.gameObject);
    }

    public void SetFromOptions(List<ModuleType> possibleWeaponModules)
    {
        type = RandomHelper.GetRandomFromList(possibleWeaponModules);

        text.text = type.ToString();
        text.color = material.color;

        SetMaterial();
    }
}
