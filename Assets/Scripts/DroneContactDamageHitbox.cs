using UnityEngine;

public class DroneContactDamageHitbox : MonoBehaviour
{
    private float damage;
    private float sameTargetCD;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();
    private LayerMask enemyLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip contactHit;

    public void Set(float damage, float tickSpeed)
    {
        // Get LayerMask
        enemyLayer = LayerMask.GetMask("Enemy");

        this.damage = damage;
        this.sameTargetCD = tickSpeed;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, enemyLayer)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        hb.Damage(damage, ModuleType.DRONE_CONTACT_DAMAGE);
        sameTargetCDDictionary.Add(other.gameObject, sameTargetCD);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(contactHit);
    }

    private void Update()
    {
        sameTargetCDDictionary.Update();
    }
}
