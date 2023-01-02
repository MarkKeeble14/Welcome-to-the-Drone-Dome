using UnityEngine;

public abstract class DroneModule : MonoBehaviour
{
    public abstract ModuleType Type { get; }
    public abstract ModuleCategory Category { get; }
}
