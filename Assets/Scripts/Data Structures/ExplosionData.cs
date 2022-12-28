using UnityEngine;

[System.Serializable]
public class ExplosionData
{
    public float Radius = 5f;
    public float Power = 500f;
    public float Lift = 50f;
    public float Damage;
    public LayerMask DealDamageTo;
    public GameObject VisualEffect;
    public AudioClip AudioEffect;
}
