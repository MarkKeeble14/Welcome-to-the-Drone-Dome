﻿using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager _Instance { get; private set; }

    public static Controls _Controls { get; private set; }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
            return;
        }

        // Set instance
        _Instance = this;

        // Set controls
        _Controls = new Controls();
        EnableControls();
    }

    public static void EnableControls()
    {
        _Controls.Player.Move.Enable();

        _Controls.Player.TestBinding1.Enable();
        _Controls.Player.TestBinding2.Enable();
        _Controls.Player.TestBinding3.Enable();
        _Controls.Player.TestBinding4.Enable();
        _Controls.Player.TestBinding5.Enable();

        _Controls.Player.LeftMouseClick.Enable();
        _Controls.Player.MousePosition.Enable();

        _Controls.Player.CycleDroneMode.Enable();

        _Controls.Player.Dash.Enable();

        _Controls.Player.Jump.Enable();
        _Controls.Player.Crouch.Enable();
    }
}