using UnityEngine;

public class PlayerOrbitPlaceGrid : OrbitPlacementGrid
{
    [SerializeField] private StatModifierUpgradeNode playerOrbitDistanceUpgradeNode;

    private void Start()
    {
        playerOrbitDistanceUpgradeNode.OnPurchase += SetPositions;
    }
}
