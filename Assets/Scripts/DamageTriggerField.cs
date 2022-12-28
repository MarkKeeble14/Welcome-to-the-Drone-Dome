using System.Collections;
using UnityEngine;

public class DamageTriggerField : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private float damage = 0.25f;
    [SerializeField] private float sameTargetCD = 0.1f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private LayerMask dealDamageTo;
    private TimerDictionary<GameObject> sameTargetCDDictionary = new TimerDictionary<GameObject>();
    [SerializeField] private float growSpeed = 20f;
    protected bool reachedMaxRadius;

    protected void Start()
    {
        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(duration);

        StopAllCoroutines();

        StartCoroutine(Fade());
    }

    public void SetRadius(float radius)
    {
        StartCoroutine(Grow(radius));
    }


    private IEnumerator Grow(float radius)
    {
        while (transform.localScale.x != radius)
        {
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, Vector3.one * radius, growSpeed * Time.deltaTime);

            yield return null;
        }

        reachedMaxRadius = true;
    }

    private IEnumerator Fade()
    {
        reachedMaxRadius = false;
        while (transform.localScale != Vector3.zero)
        {
            transform.localScale
                = Vector3.MoveTowards(transform.localScale, Vector3.zero, growSpeed * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, dealDamageTo)) return;
        if (sameTargetCDDictionary.ContainsKey(other.gameObject)) return;

        HealthBehaviour hb = other.gameObject.GetComponent<HealthBehaviour>();
        hb.Damage(damage);
        sameTargetCDDictionary.Add(other.gameObject, sameTargetCD);
    }

    protected void Update()
    {
        sameTargetCDDictionary.Update();
    }
}
