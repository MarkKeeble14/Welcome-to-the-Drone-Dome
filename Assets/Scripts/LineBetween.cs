using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBetween : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float duration;

    public void Reset()
    {
        StopAllCoroutines();
        lineRenderer.positionCount = 0;
    }

    public void Set(Vector3 start, Vector3 end, Action onDurationEnd)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        StartCoroutine(Lifetime(onDurationEnd));
    }

    public void Set(List<Vector3> points, Action onDurationEnd)
    {
        lineRenderer.positionCount = 1;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
            lineRenderer.positionCount++;
        }
        StartCoroutine(Lifetime(onDurationEnd));
    }

    public void Set(List<Transform> bodies, Action onDurationEnd)
    {
        lineRenderer.positionCount = 1;
        for (int i = 0; i < bodies.Count; i++)
        {
            lineRenderer.SetPosition(i, bodies[i].position);

            // If we're not on the last body, make room for another entry
            if (i < bodies.Count - 1)
                lineRenderer.positionCount++;
        }
        StartCoroutine(Lifetime(onDurationEnd));
    }

    private IEnumerator Lifetime(Action onDurationEnd)
    {
        yield return new WaitForSeconds(duration);

        onDurationEnd?.Invoke();
    }
}
