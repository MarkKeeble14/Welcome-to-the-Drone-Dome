using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTargetXZ : MonoBehaviour
{
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}
