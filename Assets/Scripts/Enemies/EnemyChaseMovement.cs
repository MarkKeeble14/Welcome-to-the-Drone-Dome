using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseMovement : MonoBehaviour
{
    [SerializeField] private EnemyDetectPlayer detectPlayer;
    [SerializeField] private float chaseSpeed;

    // Update is called once per frame
    void Update()
    {
        if (detectPlayer.Target != null)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, detectPlayer.Target.position, Time.deltaTime * chaseSpeed);
        }
    }
}
