using UnityEngine;

public abstract class Scavengeable : MonoBehaviour
{
    [SerializeField] protected SimpleObjectType objectPoolerPrefabKey;
    public abstract void OnPickup();

    public virtual void ReleaseToPool()
    {
        ObjectPooler._Instance.ReleaseSimpleObject(objectPoolerPrefabKey, gameObject);
    }
}
