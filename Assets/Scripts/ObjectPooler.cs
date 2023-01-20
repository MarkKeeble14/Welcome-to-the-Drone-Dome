using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }

        // Set instance
        _Instance = this;
    }

    [SerializeField] private SerializableDictionary<ModuleType, Projectile> moduleProjectiles = new SerializableDictionary<ModuleType, Projectile>();
    private Dictionary<ModuleType, ObjectPool<Projectile>> projectilePools = new Dictionary<ModuleType, ObjectPool<Projectile>>();

    [SerializeField]
    private SerializableDictionary<DamageTriggerFieldType, DamageTriggerField> damageTriggerFields
        = new SerializableDictionary<DamageTriggerFieldType, DamageTriggerField>();
    private Dictionary<DamageTriggerFieldType, ObjectPool<DamageTriggerField>> damageTriggerPools
        = new Dictionary<DamageTriggerFieldType, ObjectPool<DamageTriggerField>>();

    [SerializeField]
    private SerializableDictionary<SimpleObjectType, GameObject> simpleObjectPrefabs
        = new SerializableDictionary<SimpleObjectType, GameObject>();
    private Dictionary<SimpleObjectType, ObjectPool<GameObject>> simpleObjectPools = new Dictionary<SimpleObjectType, ObjectPool<GameObject>>();

    public static ObjectPool<PopupText> popupTextPool;
    [SerializeField] private PopupText popupText;

    public static ObjectPool<ThumpRing> thumpRingPool;
    [SerializeField] private ThumpRing thumpRing;

    public static ObjectPool<LineBetween> chainLightningPool;
    [SerializeField] private LineBetween chainLightning;

    public static ObjectPool<LineBetween> teslaArcPool;
    [SerializeField] private LineBetween teslaArc;

    public static ObjectPool<LineBetween> laserBeamPool;
    [SerializeField] private LineBetween laserBeam;


    private void Start()
    {
        CreateThumpRingPool();
        CreateTeslaArcPool();
        CreateChainLightningPool();
        CreateLaserBeamPool();
        CreatePopupTextPool();
    }

    private void CreateThumpRingPool()
    {
        thumpRingPool = new ObjectPool<ThumpRing>(() =>
        {
            return Instantiate(thumpRing, transform);
        }, ring =>
        {
            ring.gameObject.SetActive(true);
        }, ring =>
        {
            ring.gameObject.SetActive(false);
        }, ring =>
        {
            Destroy(ring.gameObject);
        }, false, 100);
    }

    private void CreateTeslaArcPool()
    {
        teslaArcPool = new ObjectPool<LineBetween>(() =>
        {
            return Instantiate(teslaArc, transform);
        }, arc =>
        {
            arc.gameObject.SetActive(true);
        }, arc =>
        {
            arc.gameObject.SetActive(false);
        }, arc =>
        {
            Destroy(arc.gameObject);
        }, false, 100);
    }


    private void CreateChainLightningPool()
    {
        chainLightningPool = new ObjectPool<LineBetween>(() =>
        {
            return Instantiate(chainLightning, transform);
        }, lightning =>
        {
            lightning.gameObject.SetActive(true);
        }, lightning =>
        {
            lightning.Reset();
            lightning.gameObject.SetActive(false);
        }, lightning =>
        {
            Destroy(lightning.gameObject);
        }, false, 100);
    }

    private void CreateLaserBeamPool()
    {
        laserBeamPool = new ObjectPool<LineBetween>(() =>
        {
            return Instantiate(laserBeam, transform);
        }, laser =>
        {
            laser.gameObject.SetActive(true);
        }, laser =>
        {
            laser.gameObject.SetActive(false);
        }, laser =>
        {
            Destroy(laser.gameObject);
        }, false, 100);
    }

    private void CreatePopupTextPool()
    {
        popupTextPool = new ObjectPool<PopupText>(() =>
        {
            return Instantiate(popupText, transform);
        }, text =>
        {
            text.gameObject.SetActive(true);
        }, text =>
        {
            text.gameObject.SetActive(false);
        }, text =>
        {
            Destroy(text.gameObject);
        }, true, 100);
    }

    public ObjectPool<Projectile> GetProjectilePool(ModuleType source)
    {
        if (projectilePools.ContainsKey(source))
        {
            return projectilePools[source];
        }
        else
        {
            Projectile projectilePrefab = moduleProjectiles.GetEntry(source).Value;
            projectilePools.Add(source,
                 new ObjectPool<Projectile>(() =>
                 {
                     return Instantiate(projectilePrefab, transform);
                 }, projectile =>
                 {
                     projectile.gameObject.SetActive(true);
                 }, projectile =>
                 {
                     projectile.gameObject.SetActive(false);
                 }, projectile =>
                 {
                     Destroy(projectile.gameObject);
                 }, true, 10));
            return projectilePools[source];
        }
    }

    public Projectile GetProjectile(ModuleType source)
    {
        return GetProjectilePool(source).Get();
    }

    public void ReleaseProjectile(ModuleType source, Projectile p)
    {
        GetProjectilePool(source).Release(p);
    }

    public ObjectPool<GameObject> GetSimpleObjectPool(SimpleObjectType type)
    {
        if (simpleObjectPools.ContainsKey(type))
        {
            return simpleObjectPools[type];
        }
        else
        {
            GameObject obj = simpleObjectPrefabs.GetEntry(type).Value;
            simpleObjectPools.Add(type,
                 new ObjectPool<GameObject>(() =>
                 {
                     return Instantiate(obj, transform);
                 }, item =>
                 {
                     item.SetActive(true);
                 }, item =>
                 {
                     item.SetActive(false);
                 }, item =>
                 {
                     Destroy(item);
                 }, true, 100));
            return simpleObjectPools[type];
        }
    }

    public GameObject GetSimpleObject(SimpleObjectType type)
    {
        return GetSimpleObjectPool(type).Get();
    }

    public void ReleaseSimpleObject(SimpleObjectType type, GameObject obj)
    {
        GetSimpleObjectPool(type).Release(obj);
    }

    public ObjectPool<DamageTriggerField> GetDamageTriggerFieldPool(DamageTriggerFieldType type)
    {
        if (damageTriggerPools.ContainsKey(type))
        {
            return damageTriggerPools[type];
        }
        else
        {
            DamageTriggerField obj = damageTriggerFields.GetEntry(type).Value;
            damageTriggerPools.Add(type,
                 new ObjectPool<DamageTriggerField>(() =>
                 {
                     return Instantiate(obj, transform);
                 }, field =>
                 {
                     field.gameObject.SetActive(true);
                 }, field =>
                 {
                     field.gameObject.SetActive(false);
                 }, field =>
                 {
                     Destroy(field.gameObject);
                 }, true, 100));
            return damageTriggerPools[type];
        }
    }

    public DamageTriggerField GetDamageTriggerField(DamageTriggerFieldType type)
    {
        return GetDamageTriggerFieldPool(type).Get();
    }

    public void ReleaseDamageTriggerField(DamageTriggerFieldType type, DamageTriggerField field)
    {
        GetDamageTriggerFieldPool(type).Release(field);
    }
}
