using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCDDisplay : MonoBehaviour
{
    [SerializeField] private Bar bar;
    [SerializeField] private GameObject container;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private BoolSwitchUpgradeNode playerDashUnlocked;

    // Update is called once per frame
    void Update()
    {
        if (playerDashUnlocked.Active)
        {
            container.SetActive(true);
            bar.HardSetBar(1 - (playerMovement.CurrentDashCooldown / playerMovement.DashCooldown));
        }
        else
        {
            container.SetActive(false);
        }
    }
}
