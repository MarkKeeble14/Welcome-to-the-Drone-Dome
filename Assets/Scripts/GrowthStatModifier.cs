using UnityEngine;

[System.Serializable]
public class GrowthStatModifier : StatModifier
{
    [Header("Growth")]
    [SerializeField] private float baseGrowth = 1;
    [SerializeField] private float growth;
    public float CurrentGrowth => growth;
    [SerializeField] private float growthIncreaseUponGrow;
    [SerializeField] private StatMathOperation growthChangeBy;
    public StatMathOperation GrowthChangeBy => growthChangeBy;


    public override bool WithinBounds
    {
        get
        {
            if (growth > 0)
            {
                if (hasMax && Value + growth > maxValue)
                    return false;
            }
            else if (growth < 0)
            {
                if (hasMin && Value - growth < minValue)
                    return false;
            }
            return true;
        }
    }

    public new void Reset()
    {
        growth = baseGrowth;

        base.Reset();
    }


    public void Grow()
    {
        AddEffect(growth);

        // Scale Growth
        switch (growthChangeBy)
        {
            case StatMathOperation.ADD:
                growth += growthIncreaseUponGrow;
                break;
            case StatMathOperation.MULT:
                growth *= growthIncreaseUponGrow;
                break;
        }
    }
}
