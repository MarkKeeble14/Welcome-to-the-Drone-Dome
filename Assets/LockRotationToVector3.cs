using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotationToVector3 : MonoBehaviour
{
    [SerializeField] private Vector3 vector;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(vector);
    }
}
