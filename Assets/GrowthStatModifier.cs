using UnityEngine;

[CreateAssetMenu(fileName = "GrowthStatModifier", menuName = "StatModifiers/GrowthStatModifier")]
public class GrowthStatModifier : StatModifier
{
    [Header("Growth")]
    [SerializeField] private float baseGrowth;
    [SerializeField] private float growth;
    [SerializeField] private float growthIncreaseUponGrow;
    [SerializeField] private StatMathOperation growthChangeBy;

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
