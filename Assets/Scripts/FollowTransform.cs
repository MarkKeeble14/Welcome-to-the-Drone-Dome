using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform t;
    [SerializeField] private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        if (t == null) return;
        transform.position = t.transform.position + offset;
    }
}
