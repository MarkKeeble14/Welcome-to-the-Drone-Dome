using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private StatModifier moveSpeed;
    [SerializeField] private StatModifier dashDuration;
    [SerializeField] private StatModifier dashSpeed;
    [SerializeField] private StatModifier dashCDStart;

    [SerializeField] private float crouchingDampenSpeed = .5f;
    [SerializeField] private float hoveringDampenSpeed = 1f;
    [SerializeField] private float hoveringHeight = 2f;
    [SerializeField] private float baseHeight = 0.5f;
    [SerializeField] private float ascensionSpeed = 10f;
    [SerializeField] private float descensionSpeed = 7.5f;
    [SerializeField] private AnimationCurve ascensionCurve;
    [SerializeField] private AnimationCurve descensionCurve;

    [SerializeField] private bool enableYAxis;

    private PlayerYAxisState state = PlayerYAxisState.STANDING;

    private Vector2 moveVector;
    private Rigidbody rb;

    [SerializeField] private BoolSwitchUpgradeNode allowPlayerDash;
    public float DashCooldown => dashCDStart.Value;
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
    private float MoveSpeed
    {
        get
        {
            if (!enableYAxis) return moveSpeed.Value;
            switch (state)
            {
                case PlayerYAxisState.CROUCHING:
                    return moveSpeed.Value * crouchingDampenSpeed;
                case PlayerYAxisState.IN_AIR:
                    return moveSpeed.Value * hoveringDampenSpeed;
                default:
                    return moveSpeed.Value;
            }
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
        Vector2 dashVector = InputManager._Controls.Player.Move.ReadValue<Vector2>().normalized * dashDuration.Value;
        StopAllCoroutines();

        StartCoroutine(ExecuteDash(dashVector));
    }

    private IEnumerator ExecuteDash(Vector2 vector)
    {
        dashCDTimer = dashCDStart.Value;

        float t = 0;

        while (t < dashDuration.Value)
        {
            // overrideControl = true;
            Vector3 targetPos = new Vector3(vector.x, 0, vector.y) +
                new Vector3(transform.position.x, TargetY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos,
                moveSpeed.Value * dashSpeed.Value * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

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

        // Move player accordingly
        transform.position += (MoveSpeed * Time.deltaTime) * new Vector3(moveVector.x, 0, moveVector.y);
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
