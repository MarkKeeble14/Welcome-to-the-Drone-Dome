using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashCDDisplay : MonoBehaviour
{
    [SerializeField] private Bar bar;
    [SerializeField] private GameObject container;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private BoolSwitchUpgradeNode playerDashUnlocked;
    [SerializeField] private TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        if (playerDashUnlocked.Active)
        {
            container.SetActive(true);
            float cooldownPercent = 1 - (playerMovement.CurrentDashCooldown / playerMovement.DashCooldown);
            bar.HardSetBar(cooldownPercent);
            text.gameObject.SetActive(cooldownPercent >= 1 ? true : false);
        }
        else
        {
            container.SetActive(false);
        }
    }
}
