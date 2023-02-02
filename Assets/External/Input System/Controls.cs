//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/External/Input System/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""266e0c1b-93ae-42b2-8fee-d5d6e8af3eef"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3d8dbe2c-8392-401b-930b-ad4e4f37ad87"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TestBinding1"",
                    ""type"": ""Button"",
                    ""id"": ""d4c86cb0-2fdd-4fb2-9de9-7789bb706492"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TestBinding2"",
                    ""type"": ""Button"",
                    ""id"": ""a52c544e-8b93-48a7-b42c-a95822e9a97a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TestBinding3"",
                    ""type"": ""Button"",
                    ""id"": ""6a0ed0ce-74d3-4e1a-bd9a-be093f8626d0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TestBinding4"",
                    ""type"": ""Button"",
                    ""id"": ""2b6119bd-e6a6-4f8e-958d-7aee015a6846"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TestBinding5"",
                    ""type"": ""Button"",
                    ""id"": ""aee5e94f-e096-4c6a-ad9c-61a0c4018be4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""465aa6e6-4e1f-4cfd-8e11-893e1ac9b185"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""699f60a6-dc2c-4be0-8771-0ac44e05e789"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CycleDroneMode"",
                    ""type"": ""Button"",
                    ""id"": ""fd1a3b58-8b6b-404c-8bc1-2f09e067386b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""6a35010f-913e-4cb1-9f1d-b45fc120091e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""01434632-dffd-4126-8768-53a80b3f2ca0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""adfd73b2-3880-4b1c-bbf0-664ef3b4ac9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Active"",
                    ""type"": ""Button"",
                    ""id"": ""d1eabed5-14ea-4ee3-b756-b10f4bb75465"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BeginArena"",
                    ""type"": ""Button"",
                    ""id"": ""396f37b6-c450-4d64-82cf-a0070fe964f0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NextLevel"",
                    ""type"": ""Button"",
                    ""id"": ""c0b0f191-8492-4b8b-8e58-7359e3a58ee5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Win"",
                    ""type"": ""Button"",
                    ""id"": ""e07a5e32-0544-4cf2-a36c-92a11ea99a85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""accb2611-ab3a-4619-8866-9fce4f36c228"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TestBinding9"",
                    ""type"": ""Button"",
                    ""id"": ""aecb99fa-18f3-4d86-ac4f-90622010e2fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""3138472a-292c-4cb6-b9e1-f3b4321411e7"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""97949b62-10bc-42a3-8e3a-0fb87b26144d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9565abc4-c92a-4968-8746-67b1575b7bcc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ea80d803-f012-4583-b40d-f4aad637501d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""af37fab8-4df4-4df1-8ec2-2d9f97b13473"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""84d7fc64-3afd-4b15-854e-dbb1358118a9"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestBinding1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d492cfc0-90ad-4cc2-a70d-d00db2b1b5f1"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestBinding2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d21106b-2239-471d-98db-76b67e21a089"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestBinding3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff1def89-3425-428d-bb01-89a15a31c4bb"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestBinding4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""032258f6-caee-4e22-ad83-e43b185cb512"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestBinding5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e7c1931-93a8-4a43-9800-58725f91ee10"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89c88677-dc83-4840-8ff4-b4772e6fdbc4"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2fb28c82-e2f0-4fb8-9b4e-6202b32f5f97"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CycleDroneMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1476e066-3a6b-4fe4-94aa-538b26658348"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff1b2723-61a8-4d66-86ff-23610d74dfe3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cc141b3-f379-4ffa-b8bf-ca11060a4465"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Active"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92e80ba5-ce0a-447e-b4ab-e4e8c3235f13"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BeginArena"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55bce4be-5ca5-437b-be05-ed277bc1a8ce"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextLevel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5366f3f9-4037-4b7f-bd3a-2310adb46ff1"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Win"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ff5f2e5-cd8f-4073-95d8-eabbcaa50c0a"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec12328b-9447-4b02-add1-46d92a617668"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TestBinding9"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_TestBinding1 = m_Player.FindAction("TestBinding1", throwIfNotFound: true);
        m_Player_TestBinding2 = m_Player.FindAction("TestBinding2", throwIfNotFound: true);
        m_Player_TestBinding3 = m_Player.FindAction("TestBinding3", throwIfNotFound: true);
        m_Player_TestBinding4 = m_Player.FindAction("TestBinding4", throwIfNotFound: true);
        m_Player_TestBinding5 = m_Player.FindAction("TestBinding5", throwIfNotFound: true);
        m_Player_LeftMouseClick = m_Player.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Player_MousePosition = m_Player.FindAction("MousePosition", throwIfNotFound: true);
        m_Player_CycleDroneMode = m_Player.FindAction("CycleDroneMode", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_Active = m_Player.FindAction("Active", throwIfNotFound: true);
        m_Player_BeginArena = m_Player.FindAction("BeginArena", throwIfNotFound: true);
        m_Player_NextLevel = m_Player.FindAction("NextLevel", throwIfNotFound: true);
        m_Player_Win = m_Player.FindAction("Win", throwIfNotFound: true);
        m_Player_Pause = m_Player.FindAction("Pause", throwIfNotFound: true);
        m_Player_TestBinding9 = m_Player.FindAction("TestBinding9", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_TestBinding1;
    private readonly InputAction m_Player_TestBinding2;
    private readonly InputAction m_Player_TestBinding3;
    private readonly InputAction m_Player_TestBinding4;
    private readonly InputAction m_Player_TestBinding5;
    private readonly InputAction m_Player_LeftMouseClick;
    private readonly InputAction m_Player_MousePosition;
    private readonly InputAction m_Player_CycleDroneMode;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_Active;
    private readonly InputAction m_Player_BeginArena;
    private readonly InputAction m_Player_NextLevel;
    private readonly InputAction m_Player_Win;
    private readonly InputAction m_Player_Pause;
    private readonly InputAction m_Player_TestBinding9;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @TestBinding1 => m_Wrapper.m_Player_TestBinding1;
        public InputAction @TestBinding2 => m_Wrapper.m_Player_TestBinding2;
        public InputAction @TestBinding3 => m_Wrapper.m_Player_TestBinding3;
        public InputAction @TestBinding4 => m_Wrapper.m_Player_TestBinding4;
        public InputAction @TestBinding5 => m_Wrapper.m_Player_TestBinding5;
        public InputAction @LeftMouseClick => m_Wrapper.m_Player_LeftMouseClick;
        public InputAction @MousePosition => m_Wrapper.m_Player_MousePosition;
        public InputAction @CycleDroneMode => m_Wrapper.m_Player_CycleDroneMode;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @Active => m_Wrapper.m_Player_Active;
        public InputAction @BeginArena => m_Wrapper.m_Player_BeginArena;
        public InputAction @NextLevel => m_Wrapper.m_Player_NextLevel;
        public InputAction @Win => m_Wrapper.m_Player_Win;
        public InputAction @Pause => m_Wrapper.m_Player_Pause;
        public InputAction @TestBinding9 => m_Wrapper.m_Player_TestBinding9;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @TestBinding1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding1;
                @TestBinding1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding1;
                @TestBinding1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding1;
                @TestBinding2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding2;
                @TestBinding2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding2;
                @TestBinding2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding2;
                @TestBinding3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding3;
                @TestBinding3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding3;
                @TestBinding3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding3;
                @TestBinding4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding4;
                @TestBinding4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding4;
                @TestBinding4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding4;
                @TestBinding5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding5;
                @TestBinding5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding5;
                @TestBinding5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding5;
                @LeftMouseClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @MousePosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMousePosition;
                @CycleDroneMode.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCycleDroneMode;
                @CycleDroneMode.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCycleDroneMode;
                @CycleDroneMode.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCycleDroneMode;
                @Dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Crouch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Active.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnActive;
                @Active.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnActive;
                @Active.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnActive;
                @BeginArena.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBeginArena;
                @BeginArena.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBeginArena;
                @BeginArena.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBeginArena;
                @NextLevel.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextLevel;
                @NextLevel.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextLevel;
                @NextLevel.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextLevel;
                @Win.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWin;
                @Win.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWin;
                @Win.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWin;
                @Pause.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @TestBinding9.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding9;
                @TestBinding9.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding9;
                @TestBinding9.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTestBinding9;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @TestBinding1.started += instance.OnTestBinding1;
                @TestBinding1.performed += instance.OnTestBinding1;
                @TestBinding1.canceled += instance.OnTestBinding1;
                @TestBinding2.started += instance.OnTestBinding2;
                @TestBinding2.performed += instance.OnTestBinding2;
                @TestBinding2.canceled += instance.OnTestBinding2;
                @TestBinding3.started += instance.OnTestBinding3;
                @TestBinding3.performed += instance.OnTestBinding3;
                @TestBinding3.canceled += instance.OnTestBinding3;
                @TestBinding4.started += instance.OnTestBinding4;
                @TestBinding4.performed += instance.OnTestBinding4;
                @TestBinding4.canceled += instance.OnTestBinding4;
                @TestBinding5.started += instance.OnTestBinding5;
                @TestBinding5.performed += instance.OnTestBinding5;
                @TestBinding5.canceled += instance.OnTestBinding5;
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @CycleDroneMode.started += instance.OnCycleDroneMode;
                @CycleDroneMode.performed += instance.OnCycleDroneMode;
                @CycleDroneMode.canceled += instance.OnCycleDroneMode;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Active.started += instance.OnActive;
                @Active.performed += instance.OnActive;
                @Active.canceled += instance.OnActive;
                @BeginArena.started += instance.OnBeginArena;
                @BeginArena.performed += instance.OnBeginArena;
                @BeginArena.canceled += instance.OnBeginArena;
                @NextLevel.started += instance.OnNextLevel;
                @NextLevel.performed += instance.OnNextLevel;
                @NextLevel.canceled += instance.OnNextLevel;
                @Win.started += instance.OnWin;
                @Win.performed += instance.OnWin;
                @Win.canceled += instance.OnWin;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @TestBinding9.started += instance.OnTestBinding9;
                @TestBinding9.performed += instance.OnTestBinding9;
                @TestBinding9.canceled += instance.OnTestBinding9;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnTestBinding1(InputAction.CallbackContext context);
        void OnTestBinding2(InputAction.CallbackContext context);
        void OnTestBinding3(InputAction.CallbackContext context);
        void OnTestBinding4(InputAction.CallbackContext context);
        void OnTestBinding5(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnCycleDroneMode(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnActive(InputAction.CallbackContext context);
        void OnBeginArena(InputAction.CallbackContext context);
        void OnNextLevel(InputAction.CallbackContext context);
        void OnWin(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnTestBinding9(InputAction.CallbackContext context);
    }
}
