using UnityEngine;

[System.Serializable]
public abstract class ExplosionData
{
    public abstract float Radius { get; }
    public abstract float Power { get; }
    public abstract float Lift { get; }
    public abstract float Damage { get; }
    [SerializeField] private LayerMask dealDamageTo;
    public LayerMask DealDamageTo => dealDamageTo;
    [SerializeField] private GameObject visualEffect;
    public GameObject VisualEffect => visualEffect;
    [SerializeField] private AudioClip audioEffect;
    public AudioClip AudioEffect => audioEffect;
}
