using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneModuleVisual : MonoBehaviour
{
    [SerializeField] private LayerMask collideWithLayer;
    [SerializeField] private float attachSpeed = 1f;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeSpeed;
    [SerializeField] private float shakeStrength;
    [SerializeField] private float waitAfterShake = 1f;
    [SerializeField] private float speedGainModifier = 1f;
    private DroneModule repModule;
    private new Renderer renderer;
    [SerializeField] private Material material;
    private Vector3 lockPosition;
    private bool attaching;

    [Header("Audio")]
    [SerializeField] private AudioSource shakingSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip doneShakingClip;
    [SerializeField] private AudioClip startAttachClip;
    [SerializeField] private AudioClip endAttachClip;
    private bool shaking;

    private void OnTriggerStay(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, collideWithLayer)) return;
        if (!attaching) return;
        gameObject.layer = LayerMask.NameToLayer("Drone");
        attaching = false;
    }

    private void Update()
    {
        if (!attaching && shaking)
            shakingSource.enabled = PauseManager._Instance.IsPaused ? false : true;
    }

    public void Set(DroneController drone)
    {
        repModule = GetComponentInParent<DroneModule>();
        renderer = GetComponentInChildren<Renderer>();

        // Set Variables
        material = new Material(renderer.material);
        Color color = GameManager._Instance.GetModuleColor(repModule.Type);
        material.color = color;
        renderer.material = material;

        StartCoroutine(Attach(drone));
    }

    public IEnumerator Attach(DroneController drone)
    {
        shaking = true;
        float t = 0;
        float randStartTimeX = RandomHelper.RandomFloat(0, 100f);
        float randStartTimeZ = RandomHelper.RandomFloat(0, 100f);

        // Audio
        shakingSource.pitch = RandomHelper.RandomFloat(.9f, 1.1f);
        shakingSource.enabled = true;

        // Shake
        while (t < shakeDuration)
        {
            if (Time.timeScale != 0)
            {
                t += Time.deltaTime;

                // Shake
                float xVal = Mathf.Sin(randStartTimeX + Time.time * shakeSpeed) * shakeStrength;
                float zVal = Mathf.Sin(randStartTimeZ + Time.time * shakeSpeed) * shakeStrength;
                transform.position = new Vector3(
                    transform.position.x + xVal,
                    transform.position.y,
                    transform.position.z + zVal);
            }

            yield return null;
        }

        // Audio
        shakingSource.enabled = false;
        sfxSource.pitch = RandomHelper.RandomFloat(.6f, 1.2f);
        sfxSource.PlayOneShot(doneShakingClip);

        // De-parent
        Transform parent = transform.parent;
        transform.parent = null;

        shaking = false;

        yield return new WaitForSeconds(waitAfterShake);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(startAttachClip);

        // Re-parent
        transform.parent = parent;

        attaching = true;

        // Attach
        float speedModifier = 1;
        while (attaching)
        {
            speedModifier += speedGainModifier * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, drone.transform.position, Time.deltaTime * attachSpeed * speedModifier);

            yield return null;
        }

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.7f, 1.3f);
        sfxSource.PlayOneShot(endAttachClip);

        repModule.Attached = true;
        lockPosition = transform.localPosition;
        while (true)
        {
            transform.localPosition = lockPosition;
            transform.rotation = Quaternion.identity;
            yield return null;
        }
    }
}
