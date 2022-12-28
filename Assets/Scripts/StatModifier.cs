using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatModifier", menuName = "StatModifiers/BasicStatModifier")]
public class StatModifier : ScriptableObject
{
    [SerializeField] private float baseValue;
    public float BaseValue => baseValue;
    [SerializeField] private float affectedValue;
    public float Value => affectedValue;
    [SerializeField] private float minValue;
    [SerializeField] private bool hasMin;
    [SerializeField] private float maxValue;
    [SerializeField] private bool hasMax;

    [SerializeField] private StatMathOperation changeBy;
    private List<float> affectingValue = new List<float>();

    public void AddEffect(float x)
    {
        affectingValue.Add(x);
        DetermineValue();
    }

    public void RemoveEffect(float x)
    {
        affectingValue.Remove(x);
        DetermineValue();
    }

    public void DetermineValue()
    {
        affectedValue = baseValue;
        foreach (float x in affectingValue)
        {
            AlterValue(x);
        }
    }

    public void Reset()
    {
        // Reset list
        affectingValue.Clear();

        // Re-determine Value
        DetermineValue();
    }

    private void AlterValue(float x)
    {
        switch (changeBy)
        {
            case StatMathOperation.ADD:
                Add(x);
                break;
            case StatMathOperation.MULT:
                Multiply(x);
                break;
            default:
                break;
        }
    }

    private void Add(float x)
    {
        if (x > 0)
        {
            if (hasMax)
            {
                if (affectedValue + x < maxValue)
                {
                    affectedValue += x;
                }
                else
                {
                    affectedValue = maxValue;
                }
            }
            else
            {
                affectedValue += x;
            }
        }
        else if (x < 0)
        {
            if (hasMin)
            {
                if (affectedValue - x < minValue)
                {
                    affectedValue -= x;
                }
                else
                {
                    affectedValue = minValue;
                }
            }
            else
            {
                affectedValue -= x;
            }
        }
    }

    private void Multiply(float x)
    {
        if (affectedValue * x > maxValue)
        {
            affectedValue = maxValue;
        }
        else if (affectedValue * x < minValue)
        {
            affectedValue = minValue;
        }
        else if (affectedValue * x < maxValue && affectedValue * x > minValue)
        // Logically unsure if we need the strict condition or not, might be fine to just
        // leave this an an "else"
        {
            affectedValue *= x;
        }
    }
}
