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

    protected bool primed;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip primedClip;
    [SerializeField] private AudioClip dePrimedClip;

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

    public void SetPrimed(bool v)
    {
        if (v != primed)
            primed = v;

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        if (primed)
            sfxSource.PlayOneShot(primedClip);
        else
            sfxSource.PlayOneShot(dePrimedClip);
    }
}
