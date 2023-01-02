using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroneDisplayUnit : MonoBehaviour
{
    [System.Serializable]
    public struct DroneModeDisplayInfo
    {
        public Color Color;
        public Sprite Sprite;
    }
    [SerializeField] private Image[] changeColorOnSelect;
    [SerializeField] private Image showActiveImage;
    [SerializeField] private Image showActiveContainer;
    [SerializeField] private Image droneModePic;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI indexText;
    public bool Selected;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;
    [SerializeField] private Color activeCDColor;
    [SerializeField] private Color hasActiveColor;

    private DroneController representingDrone;

    [SerializeField]
    private SerializableDictionary<DroneMode, DroneModeDisplayInfo> droneModeDisplayDictionary
        = new SerializableDictionary<DroneMode, DroneModeDisplayInfo>();

    public void Set(DroneController drone, PlayerDroneController playerDroneController, int index)
    {
        // Debug.Log("Added UI Unit for: " + drone.name);
        representingDrone = drone;
        button.onClick.AddListener(delegate { playerDroneController.TrySelectDrone(drone); });
        indexText.text = index.ToString();
    }

    public bool HasActives => representingDrone.NumActives > 0;

    private void Update()
    {
        // Control Color to display info
        if (HasActives)
        {
            showActiveContainer.gameObject.SetActive(true);
            showActiveImage.color = representingDrone.CanActivateActives ? hasActiveColor : activeCDColor;
        }
        else
        {
            showActiveContainer.gameObject.SetActive(false);
        }

        foreach (Image image in changeColorOnSelect)
        {
            image.color = Selected ? selectedColor : notSelectedColor;
        }
        SetBasedOnDroneMode();

        Vector2 cooldownPercent = representingDrone.CooldownTime;
        showActiveImage.fillAmount = cooldownPercent.x / cooldownPercent.y;
    }

    private void SetBasedOnDroneMode()
    {
        DroneModeDisplayInfo info = droneModeDisplayDictionary.GetEntry(representingDrone.CurrentMode).Value;
        // droneModePic.color = info.Color;
        droneModePic.sprite = info.Sprite;
    }
}
