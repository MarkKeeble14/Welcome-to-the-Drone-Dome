using UnityEngine;

public class SpawningPad : MonoBehaviour
{
    [SerializeField] private bool spawnable;
    public bool Spawnable
    {
        get
        {
            return spawnable;
        }
        set
        {
            spawnable = value;
        }
    }
    [SerializeField] private bool unspawnable;
    public bool Unspawnable
    {
        get
        {
            return unspawnable;
        }
        set
        {
            unspawnable = value;
        }
    }
    public bool RealSpawnable => Spawnable && !Unspawnable;
    [SerializeField] private Color defaultColor;

    private void OnEnable()
    {
        defaultColor = renderer.material.color;
    }

    private void OnDisable()
    {
        renderer.material.color = defaultColor;
    }

    public new Renderer renderer;
    private void Update()
    {
        /*
        if (RealSpawnable)
        {
            renderer.material.color = Color.green;
        }
        else if (Unspawnable)
        {
            renderer.material.color = Color.red;
        }
        else
        {
            renderer.material.color = defaultColor;
        }
        */
    }
}
