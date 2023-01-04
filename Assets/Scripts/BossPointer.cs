using UnityEngine;
using UnityEngine.UI;

public class BossPointer : MonoBehaviour
{
    private Transform player;
    private Transform target;
    [SerializeField] private float radiusDistance;

    public void Set(Transform player, GameObject gameObject)
    {
        target = gameObject.transform;
        this.player = player;
    }

    private void Update()
    {
        transform.position = SetPosition(player, target.position);
        transform.LookAt(target.position, Vector3.up);
    }

    private Vector3 SetPosition(Transform anchor, Vector3 targetPos)
    {
        Vector3 centerPosition = anchor.position; // Center position
        float distance = Vector3.Distance(targetPos, centerPosition); // Distance from anchor to position
        Vector3 position = targetPos; // Default position to mousePos; if nothing needs to change about it, it's within
                                      // the bounds already

        Vector3 fromOriginToObject = targetPos - centerPosition; // Find vector between objects
        fromOriginToObject *= radiusDistance / distance; //Multiply by radius, then Divide by distance
        position = centerPosition + fromOriginToObject; // Add new vector to anchor position

        return position;
    }
}
