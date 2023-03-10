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
    private bool selected;
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            representingDrone.selected = value;
            selected = value;
        }
    }
    public bool HasActives => representingDrone.GetNumberOfModules(ModuleCategory.ACTIVE) > 0;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color notSelectedColor;
    [SerializeField] private Color cannotActivateActiveColor;
    [SerializeField] private Color canActivateActivesColor;
    [SerializeField] private Color activeOnCDColor;

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

    private void Update()
    {
        // Control Color to display info
        if (HasActives)
        {
            showActiveContainer.gameObject.SetActive(true);

            if (!representingDrone.CanActivateActives)
            {
                if (representingDrone.CoolingDown)
                {
                    showActiveImage.color = activeOnCDColor;
                }
                else
                {
                    showActiveImage.color = cannotActivateActiveColor;
                }
            }
            else
            {
                showActiveImage.color = canActivateActivesColor;
            }

            showActiveImage.fillAmount = representingDrone.CooldownTime.x / representingDrone.CooldownTime.y;
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
    }

    private void SetBasedOnDroneMode()
    {
        DroneModeDisplayInfo info = droneModeDisplayDictionary.GetEntry(representingDrone.CurrentMode).Value;
        // droneModePic.color = info.Color;
        droneModePic.sprite = info.Sprite;
    }
}
