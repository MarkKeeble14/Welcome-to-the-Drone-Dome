using UnityEngine;

[CreateAssetMenu(fileName = "StoreSetting", menuName = "StoreSetting", order = 1)]
public class StoreSetting : ScriptableObject
{
    [Range(0, 1)]
    [SerializeField] private float setting = .8f;
    public float Value
    {
        get
        {
            return setting;
        }
        set
        {
            setting = value;
        }
    }
}
