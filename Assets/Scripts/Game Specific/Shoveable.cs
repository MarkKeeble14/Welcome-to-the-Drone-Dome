using System;
using UnityEngine;

public class Shoveable : MonoBehaviour
{
    [SerializeField] private float distanceFromPlayerBeforeDestroy = 25;

    public int ShoveRequirement = 1;

    [SerializeField] protected Rigidbody rb;
    public Action MyOnDestroy { get; set; }

    private Transform player;

    public bool Primed;

    private void OnDestroy()
    {
        MyOnDestroy();
    }

    private void Start()
    {
        player = GameManager._Instance.Player;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > distanceFromPlayerBeforeDestroy)
        {
            Destroy(gameObject);
        }
    }
}
