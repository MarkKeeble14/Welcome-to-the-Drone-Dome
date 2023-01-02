using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager _Instance { get; private set; }

    public static Controls _Controls { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            DisableControls();
            Destroy(_Instance.gameObject);
        }

        // Set instance
        _Instance = this;

        // Set controls
        _Controls = new Controls();
        EnableControls();
    }

    public static void DisableControls()
    {
        _Controls.Player.Move.Disable();

        _Controls.Player.TestBinding1.Disable();
        _Controls.Player.TestBinding2.Disable();
        _Controls.Player.TestBinding3.Disable();
        _Controls.Player.TestBinding4.Disable();
        _Controls.Player.TestBinding5.Disable();

        _Controls.Player.LeftMouseClick.Disable();
        _Controls.Player.MousePosition.Disable();

        _Controls.Player.CycleDroneMode.Disable();

        _Controls.Player.Dash.Disable();

        _Controls.Player.Jump.Disable();
        _Controls.Player.Crouch.Disable();
        _Controls.Player.Active.Disable();

        _Controls.Player.BeginArena.Disable();
        _Controls.Player.NextLevel.Disable();
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
        _Controls.Player.Active.Enable();

        _Controls.Player.BeginArena.Enable();
        _Controls.Player.NextLevel.Enable();
    }
}
