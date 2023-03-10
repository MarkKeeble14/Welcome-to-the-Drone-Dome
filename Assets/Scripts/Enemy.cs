using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float contactDamage;
    public float ContactDamage { get { return contactDamage * GameManager._Instance.EnemyStatMap.DamageMod.Value; } }

    [SerializeField] private float knockbackStrength;
    public float KnockbackStrength { get { return knockbackStrength; } }
}
