using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private BoolSwitchUpgradeNode allowPlayerDash;
    [SerializeField] private StatModifierUpgradeNode defaultMoveSpeedNode;
    public float MoveSpeed
    {
        get
        {
            if (!enableYAxis) return defaultMoveSpeedNode.Stat.Value;
            switch (state)
            {
                case PlayerYAxisState.CROUCHING:
                    return defaultMoveSpeedNode.Stat.Value * crouchingDampenSpeed;
                case PlayerYAxisState.IN_AIR:
                    return defaultMoveSpeedNode.Stat.Value * hoveringDampenSpeed;
                default:
                    return defaultMoveSpeedNode.Stat.Value;
            }
        }
    }
    [SerializeField] private StatModifierUpgradeNode defaultDashSpeedNode;
    public float DashSpeed
    {
        get
        {
            return defaultDashSpeedNode.Stat.Value;
        }
    }
    [SerializeField] private StatModifierUpgradeNode defaultDashDurationNode;
    public float DashDuration
    {
        get
        {
            return defaultDashDurationNode.Stat.Value;
        }
    }
    [SerializeField] private StatModifierUpgradeNode defaultDashCooldownNode;
    public float DashCooldown
    {
        get
        {
            return defaultDashCooldownNode.Stat.Value;
        }
    }

    [SerializeField] private float raycastCheckDistance;
    [SerializeField] private LayerMask wallLayer;

    [Header("Y Axis")]
    [SerializeField] private bool enableYAxis;
    [SerializeField] private float crouchingDampenSpeed = .5f;
    [SerializeField] private float hoveringDampenSpeed = 1f;
    [SerializeField] private float hoveringHeight = 2f;
    [SerializeField] private float baseHeight = 0.5f;
    [SerializeField] private float ascensionSpeed = 10f;
    [SerializeField] private float descensionSpeed = 7.5f;
    [SerializeField] private AnimationCurve ascensionCurve;
    [SerializeField] private AnimationCurve descensionCurve;
    private PlayerYAxisState state = PlayerYAxisState.STANDING;

    [Header("References")]
    [SerializeField] private GameObject dashParticle;

    [Header("Audio")]

    [SerializeField] private AudioSource footstepsSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip dashStartClip;
    [SerializeField] private AudioClip dashEndClip;

    public bool Dashing { get; private set; }
    private Vector2 moveVector;
    private Rigidbody rb;

    private float dashCDTimer;
    public float CurrentDashCooldown
    {
        get
        {
            if (dashCDTimer > 0)
                return dashCDTimer;
            else
                return 0;
        }
    }

    private float TargetY
    {
        get
        {
            if (!enableYAxis) return baseHeight;
            switch (state)
            {
                case PlayerYAxisState.IN_AIR:
                    return hoveringHeight;
                default:
                    return baseHeight;
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InputManager._Controls.Player.Dash.started += Dash;

        InputManager._Controls.Player.Jump.started += Jump;
        InputManager._Controls.Player.Crouch.performed += StartCrouch;
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!allowPlayerDash.Active) return;
        if (dashCDTimer > 0) return;
        Vector2 dashVector = InputManager._Controls.Player.Move.ReadValue<Vector2>().normalized;
        StopAllCoroutines();

        StartCoroutine(ExecuteDash(dashVector));
    }

    private IEnumerator ExecuteDash(Vector2 vector)
    {
        dashCDTimer = DashCooldown;

        Instantiate(dashParticle, transform);

        float t = 0;

        Dashing = true;

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(dashStartClip);

        while (t < DashDuration)
        {
            // overrideControl = true;
            Vector3 targetPos = transform.position + new Vector3(vector.x, 0, vector.y); ;

            // Check if allow move
            Vector3 direction = targetPos - transform.position;
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, wallLayer);
            if (hit.transform != null)
            {
                targetPos = hit.point;
            }

            // Move
            transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * DashSpeed * Time.deltaTime);
            if (transform.position == hit.point)
            {
                break;
            }
            t += Time.deltaTime;

            yield return null;
        }
        Dashing = false;

        // Audio
        sfxSource.pitch = RandomHelper.RandomFloat(.8f, 1.2f);
        sfxSource.PlayOneShot(dashEndClip);

        // overrideControl = false;
    }

    public void ResetDashCooldown()
    {
        dashCDTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Subtract from timer
        dashCDTimer -= Time.deltaTime;

        // Get player input then move the player accordingly
        moveVector = InputManager._Controls.Player.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(moveVector.x, 0, moveVector.y);

        // Audio
        footstepsSource.enabled = direction != Vector3.zero;

        // Move player accordingly, if not hitting wall
        RaycastHit hit;
        Physics.Raycast(transform.position, direction, out hit, raycastCheckDistance, wallLayer);
        if (hit.transform != null)
        {
            return;
        }
        transform.position += (MoveSpeed * Time.deltaTime) * direction;
    }

    private void LateUpdate()
    {
        if (!enableYAxis) return;
        if (TargetY == hoveringHeight)
        {
            rb.useGravity = false;

            // Move player to Target Y (Upwards)
            transform.position = new Vector3(transform.position.x,
                Mathf.MoveTowards(transform.position.y, TargetY, ascensionCurve.Evaluate(Time.deltaTime * ascensionSpeed)), transform.position.z);
        }
        else
        {
            if (transform.position.y == TargetY) rb.useGravity = true;

            // Move player to Target Y (Downwards)
            transform.position = new Vector3(transform.position.x,
                Mathf.MoveTowards(transform.position.y, TargetY, descensionCurve.Evaluate(Time.deltaTime * descensionSpeed)), transform.position.z);
        }
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!enableYAxis) return;
        switch (state)
        {
            case PlayerYAxisState.CROUCHING:
                state = PlayerYAxisState.STANDING;
                break;
            case PlayerYAxisState.STANDING:
                state = PlayerYAxisState.IN_AIR;
                break;
            case PlayerYAxisState.IN_AIR:
                state = PlayerYAxisState.IN_AIR;
                break;
        }
    }

    private void StartCrouch(InputAction.CallbackContext ctx)
    {
        if (!enableYAxis) return;
        switch (state)
        {
            case PlayerYAxisState.CROUCHING:
                state = PlayerYAxisState.STANDING;
                break;
            case PlayerYAxisState.STANDING:
                state = PlayerYAxisState.CROUCHING;
                break;
            case PlayerYAxisState.IN_AIR:
                state = PlayerYAxisState.STANDING;
                break;
        }
    }
}
