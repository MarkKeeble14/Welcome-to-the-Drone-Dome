using System.Collections.Generic;
using UnityEngine;

public class SpawningRadiusTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask spawningPadLayer;
    [SerializeField] private float radius;
    private Dictionary<Collider, SpawningPad> spawningPadDict = new Dictionary<Collider, SpawningPad>();
    private List<SpawningPad> spawningPads = new List<SpawningPad>();
    public List<SpawningPad> SpawningPads => spawningPads;
    public bool positiveChangeState;

    private void Awake()
    {
        transform.localScale = Vector3.one * radius;
    }

    private void Update()
    {
        transform.localScale = Vector3.one * radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, spawningPadLayer)) return;
        if (!spawningPadDict.ContainsKey(other))
        {
            spawningPadDict.Add(other, other.GetComponent<SpawningPad>());
        }
        spawningPads.Add(spawningPadDict[other]);
        if (positiveChangeState)
            spawningPadDict[other].Spawnable = true;
        else
            spawningPadDict[other].Unspawnable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, spawningPadLayer)) return;
        if (!spawningPadDict.ContainsKey(other))
        {
            spawningPadDict.Add(other, other.GetComponent<SpawningPad>());
        }
        spawningPads.Remove(spawningPadDict[other]);
        if (positiveChangeState)
            spawningPadDict[other].Spawnable = false;
        else
            spawningPadDict[other].Unspawnable = false;
    }
}
