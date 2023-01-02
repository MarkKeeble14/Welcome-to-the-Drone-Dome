using System;
using UnityEngine;

public class Shoveable : MonoBehaviour
{
    [SerializeField] private bool destroyWhenTooFarFromPlayer = true;
    [SerializeField] private float distanceFromPlayerBeforeDestroy = 25;

    public int ShoveRequirement = 1;

    [SerializeField] protected Rigidbody rb;
    public Action CallOnDestroy { get; set; }

    private Transform player;

    public bool Primed;

    private void OnDestroy()
    {
        CallOnDestroy?.Invoke();
    }

    private void Update()
    {
        if (!destroyWhenTooFarFromPlayer) return;
        // If player is null, try to grab player
        if (player == null)
        {
            player = GameManager._Instance.Player;
        };
        // If player is still null, don't try to access player and just wait to try again next frame
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.transform.position) > distanceFromPlayerBeforeDestroy)
        {
            Destroy(gameObject);
        }
    }
}
