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

    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, collideWithLayer)) return;

        gameObject.layer = LayerMask.NameToLayer("Drone");
        attaching = false;
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
        float t = 0;
        float randStartTimeX = RandomHelper.RandomFloat(0, 100f);
        float randStartTimeZ = RandomHelper.RandomFloat(0, 100f);
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

        Transform parent = transform.parent;
        transform.parent = null;

        yield return new WaitForSeconds(waitAfterShake);

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

        repModule.Attached = true;
        lockPosition = transform.localPosition;
        while (true)
        {
            transform.localPosition = lockPosition;
            yield return null;
        }
    }
}
