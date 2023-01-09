﻿using System;
using System.Collections;
using UnityEngine;

public abstract class MortarProjectile : Projectile
{
    [SerializeField] private Rigidbody rb;
    private LayerMask explodeOnCollideWith;

    private void Awake()
    {
        explodeOnCollideWith = LayerMask.GetMask("Enemy", "Ground");
    }

    public void Set(Transform shootAt, float speed, float arcAngle)
    {
        rb.velocity = Vector3.zero;
        ShootAt(shootAt, arcAngle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnCollideWith)) return;
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

        if (float.IsNaN(vel))
        {
            Debug.Log(name + " - Attempted to set Impossible Trajectory");
        }
        rb.velocity = vel * shotDir;
    }
}
