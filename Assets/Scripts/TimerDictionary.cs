using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerDictionary<T>
{
    private Dictionary<T, float> dictionary = new Dictionary<T, float>();
    public void Update()
    {
        // Find expired powerups and reduce the duration on all powerups by time.deltatime
        List<T> expired = new List<T>();

        foreach (KeyValuePair<T, float> kvp in dictionary.ToList())
        {
            dictionary[kvp.Key] -= Time.deltaTime;
            if (dictionary[kvp.Key] <= 0)
            {
                expired.Add(kvp.Key);
            }
        }

        // remove and destroy expired powerups
        foreach (T element in expired)
        {
            dictionary.Remove(element);
        }
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    public void Add(T key, float time)
    {
        dictionary.Add(key, time);
    }

    public bool ContainsKey(T key)
    {
        return dictionary.ContainsKey(key);
    }
}
