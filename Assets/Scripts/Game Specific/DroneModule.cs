using UnityEngine;

public abstract class DroneModule : MonoBehaviour
{
    public virtual ModuleType Type { get; }
    public virtual ModuleCategory Category { get; }
}
