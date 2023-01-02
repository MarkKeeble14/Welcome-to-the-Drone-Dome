using UnityEngine;

public class LineBetween : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float duration;
    private bool set;
    public void Set(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        set = true;
    }

    private void Update()
    {
        if (set)
            duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(gameObject);
    }
}
