using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPermanantUpgradeShopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if colliding with Player
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, LayerMask.GetMask("Player"))) return;
        PermanantUpgradeShopManager._Instance.Open();
    }
}
