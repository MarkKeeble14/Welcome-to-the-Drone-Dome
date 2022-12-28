using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectPlayer : MonoBehaviour
{
    public Transform Target { get; private set; }

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float sightRange;

    // Update is called once per frame
    void Update()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, sightRange, playerLayer);
        if (inRange.Length == 1)
        {
            Target = inRange[0].transform;
        }
        else
        {
            Target = null;
        }
    }
}
