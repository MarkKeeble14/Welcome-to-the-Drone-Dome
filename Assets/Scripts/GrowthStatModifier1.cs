using UnityEngine;

[System.Serializable]
public class GrowthStatModifier1 : StatModifier1
{
    [Header("Growth")]
    [SerializeField] private float baseGrowth = 1;
    [SerializeField] private float growth;
    public float CurrentGrowth => growth;
    [SerializeField] private float growthIncreaseUponGrow;
    [SerializeField] private StatMathOperation growthChangeBy;
    public StatMathOperation GrowthChangeBy => growthChangeBy;

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
