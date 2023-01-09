using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop
{
    public PercentageMap<SimpleObjectType> dropObject;
    public DropType type;
    public Vector2 chance;

    public List<GameObject> DropObjects(float dropModifier)
    {
        List<GameObject> toReturn = new List<GameObject>();
        switch (type)
        {
            case DropType.CHANCE_TO:
                if (chance.x == 0) break;
                if (RandomHelper.RandomIntExclusive(chance) <= chance.x)
                {
                    GameObject obj = ObjectPooler._Instance.GetSimpleObjectPool(dropObject.GetOption()).Get();
                    toReturn.Add(obj);
                }
                break;
            case DropType.NUM_BETWEEN:
                int numToDrop = Mathf.FloorToInt(RandomHelper.RandomIntExclusive(chance.x, chance.y) * dropModifier);
                for (int i = 0; i < numToDrop; i++)
                {
                    GameObject obj = ObjectPooler._Instance.GetSimpleObjectPool(dropObject.GetOption()).Get();
                    toReturn.Add(obj);
                }
                break;
        }
        return toReturn;
    }
}
