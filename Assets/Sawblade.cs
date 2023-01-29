using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : Projectile
{
    private LayerMask enemyLayer;
    [SerializeField] private Rigidbody rb;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();

    private float damage;
    private float tickSpeed;

    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    protected void Update()
    {
        sameTargetCDDictionary.Update();
    }

    protected virtual void HitHealthBehaviour(HealthBehaviour hb)
    {
        hb.Damage(damage, ModuleType.SAWBLADE_ACTIVE);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, enemyLayer)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        HitHealthBehaviour(hb);
        sameTargetCDDictionary.Add(other.gameObject, tickSpeed);
    }

    public void Set(float damage, float tickSpeed, float size, float range, float maxTimeAlive, float speed, Vector3 direction)
    {
        sameTargetCDDictionary.Clear();
        StopAllCoroutines();

        this.damage = damage;
        this.tickSpeed = tickSpeed;
        transform.localScale = Vector3.one * size;

        StartCoroutine(Lifetime(range, maxTimeAlive));
        StartCoroutine(Travel(speed, direction));
    }

    private IEnumerator Travel(float speed, Vector3 direction)
    {
        while (true)
        {
            rb.velocity = (speed * Time.deltaTime) * direction;

            yield return null;
        }
    }

    private IEnumerator Lifetime(float range, float maxTimeAlive)
    {
        Vector3 startPos = transform.position;

        float timer = 0;
        while (timer < maxTimeAlive && Vector3.Distance(startPos, transform.position) < range)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ReleaseAction();
    }
}
