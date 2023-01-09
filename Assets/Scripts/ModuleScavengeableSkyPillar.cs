using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleScavengeableSkyPillar : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float height = 50f;
    [SerializeField] private float changeSpeed = 2.5f;
    private bool canReach;
    [SerializeField] private float subsideBonusSpeed = 5f;
    private bool doneReach;
    private Transform storeParent;


    public void Reach()
    {
        canReach = true;
        doneReach = false;
        StartCoroutine(ExecuteReach());
    }


    public void Subside()
    {
        StartCoroutine(ExecuteSubside());
    }

    private IEnumerator ExecuteReach()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        Vector3 targetPos = transform.position + Vector3.up * height;

        while (canReach && Vector3.Distance(lineRenderer.GetPosition(1), targetPos) > 3f)
        {
            lineRenderer.SetPosition(0, transform.position);
            targetPos = transform.position + Vector3.up * height;
            lineRenderer.SetPosition(1, Vector3.Lerp(lineRenderer.GetPosition(1),
                targetPos, Time.deltaTime * changeSpeed));
            yield return null;
        }

        doneReach = true;
        // Debug.Log("Done Reach");
    }

    private void Update()
    {
        if (doneReach && canReach)
        {
            lineRenderer.SetPosition(0, transform.position);
        }
    }

    private IEnumerator ExecuteSubside()
    {
        storeParent = transform.parent;
        transform.SetParent(null);

        canReach = false;
        Vector3 targetPos = lineRenderer.GetPosition(1);
        while (Vector3.Distance(lineRenderer.GetPosition(0), targetPos) > 1f)
        {
            lineRenderer.SetPosition(0, Vector3.Lerp(lineRenderer.GetPosition(0),
                targetPos, Time.deltaTime * changeSpeed * subsideBonusSpeed));
            yield return null;
        }

        // Debug.Log("Done Subside");
        transform.SetParent(storeParent);
    }
}
