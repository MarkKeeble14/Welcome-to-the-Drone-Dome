using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstFireGunHelper : MonoBehaviour
{
    public static BurstFireGunHelper _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }

        // Set instance
        _Instance = this;
    }

    public void CallBurstFire(Action callback, float delay, float repititions)
    {
        StartCoroutine(BurstFire(callback, delay, repititions));
    }

    public IEnumerator BurstFire(Action callback, float delay, float repititions)
    {
        for (int i = 0; i < repititions; i++)
        {
            callback();
            yield return new WaitForSeconds(delay);
        }
    }
}
