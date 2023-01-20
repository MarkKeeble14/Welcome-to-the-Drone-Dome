using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectUpgradeNodeDisplay : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image typeIcon;
    private UpgradeNodeType type;

    [SerializeField] private Sprite droneIcon;
    [SerializeField] private Sprite otherIcon;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color newBackgroundColor;
    [SerializeField] private Color defaultBackgroundColor;
    [SerializeField] private Transform iconList;
    [SerializeField] private DroneModuleIcon iconPrefab;

    private void SetNew(bool v)
    {
        backgroundImage.color = v ? newBackgroundColor : defaultBackgroundColor;
    }

    public void SetDrone(DroneController drone, Action<DroneController> onClick)
    {
        type = UpgradeNodeType.DRONE;

        SetNew(drone.HasNewlyUnlockedNode);

        foreach (DroneModule module in drone.AppliedModules)
        {
            DroneModuleIcon spawned = Instantiate(iconPrefab, iconList);
            DroneModuleInfo info = GameManager._Instance.GetModuleInfo(module.Type);
            spawned.Set(info.Sprite, info.Color);
        }

        button.onClick.AddListener(delegate
        {
            onClick(drone);
        });
    }

    public void SetOther(UpgradeTree tree, Action<UpgradeTree> onClick)
    {
        type = UpgradeNodeType.OTHER;

        SetNew(tree.HasNewlyUnlockedNode);

        DroneModuleIcon spawned = Instantiate(iconPrefab, iconList);
        UpgradeTreeDisplayInfo info = GameManager._Instance.GetOtherInfo(tree.UpgradeTreeRelation);
        spawned.Set(info.Sprite, info.Color);
        otherIcon = info.Sprite;

        button.onClick.AddListener(delegate
        {
            onClick(tree);
        });
    }

    private void Update()
    {
        typeIcon.sprite = type == UpgradeNodeType.DRONE ? droneIcon : otherIcon;
    }
}
