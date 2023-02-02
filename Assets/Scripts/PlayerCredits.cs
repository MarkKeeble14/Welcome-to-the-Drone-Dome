using UnityEngine;

public class PlayerCredits
{
    private static string key = "PlayerCredits";

    public static int Get()
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key);
        }
        else
        {
            return 0;
        }
    }

    public static int Change(int amount)
    {
        if (PlayerPrefs.HasKey(key))
        {
            int result = PlayerPrefs.GetInt(key) + amount;
            if (result < 0) return 0;
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + amount);
            return result;
        }
        else
        {
            if (amount < 0) return 0;
            PlayerPrefs.SetInt(key, amount);
            return amount;
        }
    }
}
