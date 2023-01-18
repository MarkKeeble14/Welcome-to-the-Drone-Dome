using UnityEngine;

[System.Serializable]
public struct DroneModuleInfo
{
    public ModuleCategory Category;
    public Color Color;
    public Sprite Sprite;
    public int Cost;
    public bool Unobtainable;
    public DroneModule Module;
}
