using UnityEngine;

[CreateAssetMenu(fileName = "StoreInt", menuName = "StoreInt")]
public class StoreInt : ScriptableObject
{
    [SerializeField] private int value;
    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }
}
