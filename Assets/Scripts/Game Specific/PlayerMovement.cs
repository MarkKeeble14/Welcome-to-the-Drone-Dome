using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashCDStart = 1f;

    [SerializeField] private float crouchingDampenSpeed = .5f;
    [SerializeField] private float jumpingDampenSpeed = 1f;


    private float MoveSpeed
    {
        get
        {
            switch (yAxisController.State)
            {
                case PlayerYAxisState.CROUCHING:
                    return moveSpeed * crouchingDampenSpeed;
                case PlayerYAxisState.IN_AIR:
                    return moveSpeed * jumpingDampenSpeed;
                default:
                    return moveSpeed;
            }
        }
    }
    public float DashCooldown => dashCDStart;
    private Controls cachedControls;
    private float dashCDTimer;
    private bool overrideControl;

    [SerializeField] private BoolSwitchUpgradeNode allowPlayerDash;

    private PlayerYAxisController yAxisController;

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
    private Vector2 moveVector;
    [HideInInspector] public bool LockMoveVector;

    private void Awake()
    {
        // Fetch References
        yAxisController = GetComponent<PlayerYAxisController>();
    }


    private void Start()
    {
        // Cache the controls
        cachedControls = InputManager._Controls;

        cachedControls.Player.Dash.started += Dash;
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!allowPlayerDash.Active) return;
        if (dashCDTimer > 0) return;
        Vector2 dashVector = cachedControls.Player.Move.ReadValue<Vector2>().normalized * dashDistance;
        StopAllCoroutines();

        StartCoroutine(ExecuteDash(dashVector));
    }

    private IEnumerator ExecuteDash(Vector2 vector)
    {
        dashCDTimer = dashCDStart;

        Vector3 targetPos = new Vector3(vector.x, 0, vector.y) + new Vector3(transform.position.x, 0, transform.position.z);
        while (transform.position != targetPos)
        {
            overrideControl = true;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);

            yield return null;
        }

        overrideControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Subtract from timer
        dashCDTimer -= Time.deltaTime;

        // If the player is not to have control, don't allow them movement
        if (overrideControl) return;

        // Get player input then move the player accordingly
        if (!LockMoveVector)
            moveVector = cachedControls.Player.Move.ReadValue<Vector2>();
        transform.position += (MoveSpeed * Time.deltaTime) * new Vector3(moveVector.x, 0, moveVector.y);
    }
}
