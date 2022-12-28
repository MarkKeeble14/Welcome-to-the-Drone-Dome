using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PercentageMap<T>
{
    [SerializeField]
    private List<SerializableKeyValuePair<T, int>> options
        = new List<SerializableKeyValuePair<T, int>>();

    public int Count => options.Count;

    private T GetFromDictionary()
    {
        // Say a list has 2 elements, 1 for SoldierEnemy with number 50, and 1 for DroneEnemy with number 50
        // That means it's a 50/50 which one of the enemies will be spawned
        int rand = RandomHelper.RandomIntExclusive(0, 100);
        float floor = 0;
        T mostCommon = default(T);
        int largestWeight = 0;
        for (int i = 0; i < options.Count; i++)
        {
            SerializableKeyValuePair<T, int> current = options[i];
            // Debug.Log(current.Key + ", " + current.Value);

            // Keeping track of the highest weighted option to return in the case of a bogus error
            if (current.Value > largestWeight)
            {
                mostCommon = current.Key;
                largestWeight = current.Value;
            }
            // Debug.Log("To Spawn: " + current.Key + ", rand must be greater than: "
            //    + floor + " and less than: " + (floor + current.Value) + ": rand = " + rand);
            if (rand > floor && rand <= floor + current.Value)
            {
                // Debug.Log(current.Key);
                return current.Key;
            }
            floor += current.Value;
        }
        if (Count > 0)
        {
            // If there are any options, by default we return the most common
            // Debug.Log("Returning most common: " + mostCommon);
            return mostCommon;
        }
        else
        {
            // Otherwise, return default (can be null)
            // Debug.Log("Returning default: " + default(T));
            return default(T);
        }
    }

    public T GetOption()
    {
        return GetFromDictionary();
    }
}
