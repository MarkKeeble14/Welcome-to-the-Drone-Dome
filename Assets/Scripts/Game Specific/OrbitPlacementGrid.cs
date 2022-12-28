using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitPlacementGrid : MonoBehaviour
{
    [SerializeField] private Transform gridNodePrefab;
    [SerializeField] private List<Transform> gridNodes = new List<Transform>();

    [SerializeField] private float distanceFromCenter = 3f;

    [SerializeField] private float rotationSpeed = .5f;

    [SerializeField] private float droneHoverAtHeight;

    [SerializeField] private int minListSize = 1;

    public Transform AddNode()
    {
        Transform t = Instantiate(gridNodePrefab, transform);
        gridNodes.Add(t);
        SetPositions();

        return t;
    }


    public List<Transform> TryGetUnfollowedNodes(List<DroneController> followers)
    {
        // Create initial list of all nodes
        List<Transform> unfollowedNodes = new List<Transform>();
        unfollowedNodes.AddRange(gridNodes);

        // Iterate through, checking if any drone is following the current node; if any are, then we remove it from
        // the candidate list of unfollowed nodes; otherwise it will remain and be returned
        foreach (Transform t in gridNodes)
        {
            foreach (DroneController drone in followers)
            {
                if (t == drone.Follow)
                {
                    unfollowedNodes.Remove(t);
                    break;
                }
            }
        }

        return unfollowedNodes;
    }

    public Transform TryGetUnfollowedNode(List<DroneController> followers)
    {
        List<Transform> unfollowedNodes = TryGetUnfollowedNodes(followers);
        if (unfollowedNodes.Count > 0)
            return unfollowedNodes[0];
        else
            return null;
    }

    public void RemoveLastNode()
    {
        // Disallow removing a node if the list has reached the minimum list size
        if (gridNodes.Count <= minListSize) return;
        Transform toDestroy = gridNodes[gridNodes.Count - 1];

        // Remove from list
        gridNodes.Remove(toDestroy);

        // Destroy object
        Destroy(toDestroy.gameObject);

        // Reset positions
        SetPositions();
    }

    public void RemoveNode(Transform node)
    {
        // Disallow removing a node if the list has reached the minimum list size
        if (gridNodes.Count <= minListSize) return;

        // Remove from list
        gridNodes.Remove(node);

        // Destroy object
        Destroy(node.gameObject);

        // Reset positions
        SetPositions();
    }

    private void SetPositions()
    {
        for (int i = 0; i < gridNodes.Count; i++)
        {
            float angle = i * (360f / gridNodes.Count);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
            Vector3 position = transform.position + direction * distanceFromCenter;
            position.y = droneHoverAtHeight;
            gridNodes[i].position = position;
        }
    }

    void Update() => Rotate();

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
