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
    [SerializeField] private bool disallowZeroValue;
    public StatMathOperation GrowthChangeBy => growthChangeBy;

    public new void Reset()
    {
        growth = baseGrowth;

        base.Reset();
    }


    public void Grow()
    {
        AddEffect(growth);

        // If we don't allow zero values and some change would end up leaving the stat at 0, simply repeat the growth
        if (disallowZeroValue && Value == 0)
        {
            Grow();
        }

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
