using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerYAxisController : MonoBehaviour
{
    Controls cachedControls;

    private PlayerYAxisState state;
    public PlayerYAxisState State => state;
    [SerializeField] private float jumpHeight = 5f;
    private bool crouching;

    private Rigidbody rb;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cachedControls = InputManager._Controls;

        cachedControls.Player.Jump.started += Jump;
        cachedControls.Player.Crouch.performed += StartCrouch;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (state != PlayerYAxisState.IN_AIR)
        {
            state = PlayerYAxisState.IN_AIR;

            playerMovement.LockMoveVector = true;

            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Reset Out of Air
        if (state == PlayerYAxisState.IN_AIR)
        {
            state = PlayerYAxisState.STANDING;

            playerMovement.LockMoveVector = false;
        }
    }

    private void StartCrouch(InputAction.CallbackContext ctx)
    {
        if (state != PlayerYAxisState.IN_AIR)
        {
            state = PlayerYAxisState.CROUCHING;
            crouching = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Reset Out of Crouch
        if (crouching && !cachedControls.Player.Crouch.IsPressed())
        {
            state = PlayerYAxisState.STANDING;
            crouching = false;
        }

        // Update state while crouching
        if (crouching && state != PlayerYAxisState.IN_AIR)
        {
            state = PlayerYAxisState.CROUCHING;
        }
    }
}
