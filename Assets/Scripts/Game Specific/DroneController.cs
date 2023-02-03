using System;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Core")]
    public DroneMode CurrentMode;
    public bool AvailableForUse = true;
    public bool selected;

    public float ShoveStrength => droneBasics.ShoveStrength.Stat.Value;
    public float MoveSpeed => droneBasics.MoveSpeed.Stat.Value;
    public float OrbitDistance => droneBasics.OrbitDistance.Stat.Value;
    public float OrbitSpeed => droneBasics.OrbitSpeed.Stat.Value;
    public float ScavengeableSightRange => droneBasics.ScavengeableSightRange.Stat.Value;
    public float ScavengingGrabRange => droneBasics.ScavengingGrabRange.Stat.Value;
    public float ScavengingSpeedMod => droneBasics.ScavengingSpeedMod.Stat.Value;

    [Header("Orbit")]
    [SerializeField] private Transform rotateAround;
    private float orbitTimer;

    [Header("Spawning")]
    [SerializeField] private Vector2 minMaxHemisphereSpawnAt;

    [Header("Scavenging")]
    [SerializeField] private LayerMask scavengeableLayer;
    [SerializeField] private LayerMask scavengeablePriorityLayer;
    private Transform scavenging;

    [Header("Station")]
    [SerializeField] private Vector3 station;
    private bool hasStation;
    [SerializeReference] private float stationCooldownStart = 10f;
    private float stationCooldownTimer;
    [SerializeField] private float droneYTranslation = .5f;

    [SerializeField] private int activesPerDrone;
    public int ActivesPerDrone
    {
        get { return activesPerDrone; }
    }
    [SerializeField] private int passivesPerDrone;
    public int PassivesPerDrone
    {
        get { return passivesPerDrone; }
    }
    [SerializeField] private int weaponsPerDrone;
    public int WeaponsPerDrone
    {
        get { return weaponsPerDrone; }
    }

    private int weaponSlotsAdded;
    public int WeaponSlotsAdded => weaponSlotsAdded;
    private int passiveSlotsAdded;
    public int PassiveSlotsAdded => passiveSlotsAdded;
    private int activeSlotsAdded;
    public int ActiveSlotsAdded => activeSlotsAdded;

    private List<DronePassiveModule> passiveModules = new List<DronePassiveModule>();
    private List<DroneActiveModule> activeModules = new List<DroneActiveModule>();
    private List<DroneWeaponModule> weaponModules = new List<DroneWeaponModule>();

    [SerializeField] private List<ModuleType> extraDefaultModules = new List<ModuleType>();
    private List<DroneModule> appliedModules = new List<DroneModule>();
    public List<DroneModule> AppliedModules => appliedModules;
    public bool HasNewlyUnlockedNode
    {
        get
        {
            foreach (DroneModule module in appliedModules)
            {
                if (module.UpgradeTree.HasNewlyUnlockedNode)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool CanActivateActives
    {
        get
        {
            foreach (DroneActiveModule active in activeModules)
            {
                if (active.CoolingDown) return false;
                if (!active.Attached) return false;
            }
            return true;
        }
    }

    public Vector2 CooldownTime
    {
        get
        {
            float x = 0;
            float y = 0;
            foreach (DroneActiveModule active in activeModules)
            {
                x += active.CurrentCooldown;
                y += active.CooldownStart;
            }
            return new Vector2(x, y);
        }
    }

    public bool CoolingDown
    {
        get
        {
            foreach (DroneActiveModule active in activeModules)
            {
                if (active.CoolingDown) return true;
            }
            return false;
        }
    }

    [Header("References")]
    private Transform player;
    private PlayerDroneController playerDroneController;
    private PlayerHealth playerHealth;
    private DroneBasicsModule droneBasics;
    [SerializeField] private DurationBar stationCooldownBar;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material scavengingMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material unavailableMaterial;
    [SerializeField] new private Renderer renderer;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip switchToAttackClip;
    [SerializeField] private AudioClip switchToScavengeClip;
    [SerializeField] private AudioClip spawnModuleClip;
    [SerializeField] private AudioClip activateActivesClip;

    private void Start()
    {
        // Set References
        player = GameManager._Instance.Player;
        playerDroneController = player.GetComponent<PlayerDroneController>();
        playerHealth = player.GetComponent<PlayerHealth>();

        droneBasics = GetComponentInChildren<DroneBasicsModule>();

        // Spawn Station Cooldown Bar
        stationCooldownBar = Instantiate(stationCooldownBar, transform);
        stationCooldownBar.name = "StationCooldownDurationBar";
    }

    private void Update()
    {
        SetMaterial();

        orbitTimer += Time.deltaTime * OrbitSpeed;

        // Prevent Drone from attacking when unwanted
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.Paused = !AvailableForUse;
        }
        if (!AvailableForUse) return;

        stationCooldownTimer -= Time.deltaTime;

        switch (CurrentMode)
        {
            case DroneMode.ATTACK:
                HandleFollowModeLogic();
                break;
            case DroneMode.SCAVENGE:
                HandleScavengeModeLogic();
                break;
            case DroneMode.STATION:
                HandleStationModeLogic();
                break;
        }
    }

    public void AddActiveSlot()
    {
        activesPerDrone++;
        activeSlotsAdded++;
    }
    public void AddPassiveSlot()
    {
        passivesPerDrone++;
        passiveSlotsAdded++;
    }
    public void AddWeaponSlot()
    {
        weaponsPerDrone++;
        weaponSlotsAdded++;
    }

    private void SetMaterial()
    {
        if (!AvailableForUse)
        {
            renderer.material = unavailableMaterial;
        }
        else if (selected)
        {
            renderer.material = selectedMaterial;
        }
        else if (CurrentMode == DroneMode.SCAVENGE)
        {
            renderer.material = scavengingMaterial;
        }
        else
        {
            renderer.material = defaultMaterial;
        }
    }

    // Cycles the drone mode, current order is FOLLOW -> SCAVENGE -> FOLLOW
    public void CycleDroneMode()
    {
        switch (CurrentMode)
        {
            case DroneMode.ATTACK:
                CurrentMode = DroneMode.SCAVENGE;
                OnEnterScavengeMode();
                scavenging = null;
                break;
            case DroneMode.SCAVENGE:
                CurrentMode = DroneMode.ATTACK;
                OnEnterAttackMode();
                break;
            case DroneMode.STATION:
                CurrentMode = DroneMode.ATTACK;
                hasStation = false;
                OnEnterAttackMode();
                break;
        }
    }

    public void OnEnterAttackMode()
    {
        // Audio
        sfxSource.PlayOneShot(switchToAttackClip);

        // Enable all attacking types
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.OnEnterAttackMode();
        }
    }

    public void OnEnterScavengeMode()
    {
        // Audio
        sfxSource.PlayOneShot(switchToScavengeClip);

        // Disable all attacking types
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.OnEnterScavengeMode();
        }
    }

    public void OnEnterStationMode()
    {
        // Disable all attacking types
        foreach (DroneWeaponModule attackModule in weaponModules)
        {
            attackModule.OnEnterAttackMode();
        }
    }

    public int GetNumberOfModules(ModuleCategory type)
    {
        return DroneModule.GetNumModulesOfCategory(type, appliedModules);
    }

    public void SetOrbitTimer(float value)
    {
        orbitTimer = value;
    }

    public void SetRotateAround(Transform target)
    {
        rotateAround = target;
    }

    private void HandleFollowModeLogic()
    {
        RotateAroundTarget();
    }

    private void RotateAroundTarget()
    {
        if (rotateAround == null) return;
        transform.position =
            Vector3.MoveTowards(transform.position, GetOrbitPosition(), Time.deltaTime * MoveSpeed);
    }

    // Clamps the drone's position to be on a circles edge around the given anchor, with some min and max radius
    private Vector3 GetOrbitPosition()
    {
        float radius = 1 * OrbitDistance; // Outer radius
                                          // Find Direction
        float angle = orbitTimer % 360;

        Vector3 position = new Vector3(
            rotateAround.position.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
            rotateAround.position.y + droneYTranslation,
            rotateAround.position.z + Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
        return position;
    }

    private Collider[] GetUnclaimedScavengeables(Collider[] colliders)
    {
        bool targetHearts = playerHealth.CurrentHealth < playerHealth.MaxHealth;
        List<Collider> unclaimedScavengeables = new List<Collider>();
        foreach (Collider col in colliders)
        {
            if (!targetHearts && col.gameObject.name.Contains("Heart"))
            {
                continue;
            }

            unclaimedScavengeables.Add(col);
            foreach (DroneController drone in playerDroneController.TrackedDrones)
            {
                if (drone.scavenging == col.transform)
                {
                    unclaimedScavengeables.Remove(col);
                    break;
                }
            }
        }
        return unclaimedScavengeables.ToArray();
    }

    private void FindNewScavengeable()
    {
        if (player == null) return;
        // Find new Target
        // Priority; if there are priority scavengeable objects in range, we grab those first
        Collider[] inRange = Physics.OverlapSphere(player.position, ScavengeableSightRange, scavengeablePriorityLayer);
        if (inRange.Length > 0)
        {
            inRange = GetUnclaimedScavengeables(inRange);
            scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            return;
        }

        // Otherwise, we scavenge the normal priority scavengeables
        inRange = Physics.OverlapSphere(player.position, ScavengeableSightRange, scavengeableLayer);
        if (inRange.Length > 0)
        {
            inRange = GetUnclaimedScavengeables(inRange);
            scavenging = TransformHelper.GetClosestTransformToTransform(transform, inRange);
            return;
        }
        scavenging = null;
    }

    private void HandleScavengeModeLogic()
    {
        if (scavenging == null || !scavenging.gameObject.activeInHierarchy)
        {
            FindNewScavengeable();
            if (scavenging == null) HandleFollowModeLogic();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, scavenging.position,
                Time.deltaTime * MoveSpeed * ScavengingSpeedMod);
            if (Vector3.Distance(transform.position, scavenging.position) <= ScavengingSpeedMod)
            {
                // "Pick Up" Object
                scavenging.GetComponent<Scavengeable>().PickupScavengeable();

                // No longer scavenging it, will find new object
                FindNewScavengeable();
            }
        }
    }

    private void HandleStationModeLogic()
    {
        if (hasStation)
        {
            // If drone is ref of screen, remove it's station
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
            if (!onScreen)
            {
                hasStation = false;
                return;
            }

            // Move towards station
            transform.position =
                Vector3.MoveTowards(transform.position, station, Time.deltaTime * MoveSpeed);
        }
        else
        {
            HandleFollowModeLogic();
        }
    }

    public void SetStation(Vector3 pos)
    {
        if (stationCooldownTimer > 0) return;
        hasStation = true;
        station = pos + Vector3.one * droneYTranslation;

        stationCooldownTimer = stationCooldownStart;
        stationCooldownBar.SetText("Stationing");
        stationCooldownBar.Set(stationCooldownTimer);
    }

    internal void ResetStationCooldown()
    {
        stationCooldownTimer = 0;
        stationCooldownBar.Cancel();
    }

    public void ResetState()
    {
        // Set to Follow
        CurrentMode = DroneMode.ATTACK;

        // Clear Lists
        passiveModules.Clear();
        activeModules.Clear();
        weaponModules.Clear();

        // Remove all module components from this drone
        foreach (DroneModule module in appliedModules)
        {
            Destroy(module.gameObject);
        }
        appliedModules.Clear();

        // Reset has Station
        hasStation = false;

        // Add Default Modules
        foreach (ModuleType moduleType in GameManager._Instance.DefaultModules)
        {
            GameManager._Instance.AddModule(this, moduleType);
        }

        foreach (ModuleType moduleType in extraDefaultModules)
        {
            GameManager._Instance.AddModule(this, moduleType);
        }
    }


    // Takes in a prefab and returns a new instance
    public DroneModule AddModule(DroneModule module)
    {
        // Instantiate new instance of passed in module prefab
        module = Instantiate(module, transform);

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.9f, 1.1f);
        sfxSource.PlayOneShot(spawnModuleClip);

        Vector3 hemispherePos = UnityEngine.Random.onUnitSphere;
        hemispherePos.y = Mathf.Abs(hemispherePos.y);
        module.transform.position += hemispherePos * RandomHelper.RandomFloat(minMaxHemisphereSpawnAt);
        module.Set(this);

        appliedModules.Add(module);
        // Determine which list to add it to
        switch (module.Category)
        {
            case ModuleCategory.ACTIVE:
                AddActiveModule((DroneActiveModule)module);
                break;
            case ModuleCategory.PASSIVE:
                AddPassiveModule((DronePassiveModule)module);
                break;
            case ModuleCategory.WEAPON:
                AddWeaponModule((DroneWeaponModule)module);
                break;
        }
        return module;
    }

    public bool RemoveModule(DroneModule module)
    {
        if (appliedModules.Contains(module))
        {
            switch (module.Category)
            {
                case ModuleCategory.WEAPON:
                    weaponModules.Remove((DroneWeaponModule)module);
                    break;
                case ModuleCategory.ACTIVE:
                    activeModules.Remove((DroneActiveModule)module);
                    break;
                case ModuleCategory.PASSIVE:
                    passiveModules.Remove((DronePassiveModule)module);
                    break;
            }
            Destroy(module.gameObject);
            appliedModules.Remove(module);
            return true;
        }
        return false;
    }

    private void AddPassiveModule(DronePassiveModule type)
    {
        // Add the module
        passiveModules.Add(type);
    }

    private void AddActiveModule(DroneActiveModule type)
    {
        // Add the module
        activeModules.Add(type);
    }

    private void AddWeaponModule(DroneWeaponModule type)
    {
        // Add the module
        weaponModules.Add(type);

        // It can begin attacking asap if not in scavenge mode
        if (CurrentMode != DroneMode.SCAVENGE)
            type.OnAdd();
    }

    public void ActivateActives()
    {
        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(activateActivesClip);

        foreach (DroneActiveModule active in activeModules)
        {
            active.Activate();
        }
    }
    public void ResetActiveCooldown()
    {
        foreach (DroneActiveModule active in activeModules)
        {
            active.ResetCooldown();
        }
    }
}