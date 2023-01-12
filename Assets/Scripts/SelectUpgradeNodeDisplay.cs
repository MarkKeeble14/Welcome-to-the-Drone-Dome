using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectUpgradeNodeDisplay : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image typeIcon;
    private UpgradeNodeType type;

    [SerializeField] private Sprite droneIcon;
    [SerializeField] private Sprite otherIcon;

    public void SetDrone(DroneController drone, Action<DroneController> onClick)
    {
        type = UpgradeNodeType.DRONE;
        button.onClick.AddListener(delegate
        {
            onClick(drone);
        });
    }

    public void SetOther(UpgradeTree tree, Action<UpgradeTree> onClick)
    {
        type = UpgradeNodeType.OTHER;
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
