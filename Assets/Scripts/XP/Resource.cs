using System.Collections.Generic;
using UnityEngine;

public class Resource : AutoCollectScavengeable
{
    [SerializeField] private Vector2 minMaxValue = new Vector2(1, 3);
    private int Value => RandomHelper.RandomIntInclusive(minMaxValue);

    public override void PickupScavengeable()
    {
        ShopManager._Instance.AlterResource(Value);
        base.PickupScavengeable();
    }
}
