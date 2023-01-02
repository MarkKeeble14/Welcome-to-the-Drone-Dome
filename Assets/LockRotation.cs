using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    [SerializeField] private float lockX = -1;
    [SerializeField] private float lockY = -1;
    [SerializeField] private float lockZ = -1;
    [SerializeField] private bool cancelParentRotationX;
    [SerializeField] private bool cancelParentRotationY;
    [SerializeField] private bool cancelParentRotationZ;

    private void Update()
    {
        Vector3 cachedRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(
            (cancelParentRotationX ? -transform.root.eulerAngles.x : 0) + (lockX != -1 ? lockX : cachedRotation.x),
            (cancelParentRotationY ? -transform.root.eulerAngles.y : 0) + (lockY != -1 ? lockY : cachedRotation.y),
            (cancelParentRotationZ ? -transform.root.eulerAngles.z : 0) + (lockZ != -1 ? lockZ : cachedRotation.z)
            );
    }
}
