using UnityEngine;

[System.Serializable]
public abstract class ExplosionData
{
    public abstract float Radius { get; set; }
    public abstract float Power { get; set; }
    public abstract float Lift { get; set; }
    public abstract float Damage { get; set; }
    [SerializeField] private LayerMask dealDamageTo;
    public LayerMask DealDamageTo => dealDamageTo;
    [SerializeField] private GameObject visualEffect;
    public GameObject VisualEffect => visualEffect;
    [SerializeField] private AudioClip audioEffect;
    public AudioClip AudioEffect => audioEffect;
}
