using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroneDisplayUnit : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image imageBorder;
    [SerializeField] private Button button;
    public bool Selected;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;

    [SerializeField] private Color scavengeColor;
    [SerializeField] private Color followColor;

    private DroneController representingDrone;

    public void Set(DroneController drone, PlayerDroneOrbitController playerDroneOrbitController)
    {
        // Debug.Log("Added UI Unit for: " + drone.name);
        representingDrone = drone;
        button.onClick.AddListener(delegate { playerDroneOrbitController.TrySelectDrone(drone); });
    }

    private void Update()
    {
        // Control Color to display info
        imageBorder.color = Selected ? selectedColor : notSelectedColor;
        fillImage.color = GetColorBasedOnDroneMode();
    }

    private Color GetColorBasedOnDroneMode()
    {
        switch (representingDrone.CurrentMode)
        {
            case DroneMode.FOLLOW:
                return followColor;
            case DroneMode.SCAVENGE:
                return scavengeColor;
            default:
                return Color.grey;
        }
    }
}
