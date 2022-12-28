using UnityEngine;

[CreateAssetMenu(fileName = "BoolSwitchUpgradeNode", menuName = "UpgradeNode/BoolSwitchUpgradeNode")]
public class BoolSwitchUpgradeNode : UpgradeNode
{
    public bool Active;

    public override bool Maxed()
    {
        return Active;
    }

    public override void Purchase()
    {
        if (Maxed()) return;

        Active = true;
        Purchased = true;
    }

    public override void Reset()
    {
        base.Reset();
        Active = false;
    }
}
