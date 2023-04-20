using System;
using System.Collections;
using UnityEngine;

public abstract class MortarProjectile : Projectile
{
    [Header("Base Mortar Projectile")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask explodeOnCollideWith;

    [Header("Audio")]
    [SerializeField] private AudioClip contactClip;
    private bool arrived;

    public void Set(Transform shootAt, float speed, float arcAngle)
    {
        rb.velocity = Vector3.zero;
        ShootAt(shootAt, arcAngle);
        arrived = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnCollideWith)) return;
        if (arrived) return;
        arrived = true;
        AudioManager._Instance.PlayClip(contactClip, true, transform.position);
        ArrivedAtPosition();
    }

    public abstract void ArrivedAtPosition();

    private void ShootAt(Transform shootAt, float arcAngle)
    {
        Vector3 deltaPos = shootAt.position - transform.position;
        Vector3 xzDelta = deltaPos;
        xzDelta.y = 0f;
        Vector3 shotDir = Quaternion.LookRotation(xzDelta) * Quaternion.AngleAxis(-arcAngle, Vector3.right) * Vector3.forward;

        float time = Mathf.Sqrt((shotDir.y * deltaPos.x / shotDir.x - deltaPos.y) / -Physics.gravity.y * 2);
        float vel = deltaPos.x / shotDir.x / time;

        // if the attempt to assign trajectory was invalid, cancel out the projectile
        if (float.IsNaN(vel))
        {
            Debug.Log(name + " - Attempted to set Impossible Trajectory");
            ReleaseAction?.Invoke();
        } else
        {
            rb.velocity = vel * shotDir;
        }
    }
}
