using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTeslaModule : DroneWeaponModule
{
    [Header("Tesla Module")]
    [SerializeField] protected StatModifier range;
    [SerializeField] protected StatModifier delay;
    [SerializeField] protected StatModifier damage;
    private float timeSinceLastShock;
    [SerializeField] private float resetLineRendererAfter = 1f;
    private string[] canTargetLayerStrings = new string[] { "Enemy" };
    private LayerMask canTargetLayers;
    private LineRenderer lineRenderer;

    public override ModuleType Type => ModuleType.TESLA_COIL;

    private void Awake()
    {
        // Get Line Renderer Component
        lineRenderer = GetComponent<LineRenderer>();

        // Get LayerMask
        canTargetLayers = LayerMask.GetMask(canTargetLayerStrings);

        // Load Stat Modifiers
        LoadResources();
    }

    private void LoadResources()
    {
        delay = Resources.Load<StatModifier>("Tesla/Stat/TeslaDelay");
        damage = Resources.Load<StatModifier>("Tesla/Stat/TeslaDamage");
        range = Resources.Load<StatModifier>("Tesla/Stat/TeslaRange");
    }

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
    }

    public void Set()
    {
        StartAttack();
    }

    private void Update()
    {
        timeSinceLastShock += Time.deltaTime;

        // Reset Line Renderer after a certain amount of seconds
        if (timeSinceLastShock > resetLineRendererAfter
            && lineRenderer.positionCount > 0)
        {
            lineRenderer.positionCount = 0;
        }
    }

    protected IEnumerator Tesla()
    {
        yield return new WaitForSeconds(delay.Value);
        lineRenderer.positionCount = 0;

        DoDamage();
        timeSinceLastShock = 0;

        StartCoroutine(Tesla());
    }

    protected void DoDamage()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range.Value, canTargetLayers);

        if (inRange.Length == 0) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(0, transform.position);
        foreach (Collider c in inRange)
        {
            // Eventually maybe do a spark of lightning towards unit before damaging it
            lineRenderer.positionCount++;
            int numPositions = lineRenderer.positionCount;
            lineRenderer.SetPosition(numPositions - 1, c.transform.position);

            HealthBehaviour hb = null;
            if ((hb = c.GetComponent<HealthBehaviour>()) != null)
                hb.Damage(damage.Value, ModuleType.TESLA_COIL);
        }
    }

    public override void StartAttack()
    {
        StartCoroutine(Tesla());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range.Value);
    }
}
