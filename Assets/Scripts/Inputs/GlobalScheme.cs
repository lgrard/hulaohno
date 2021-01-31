// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Inputs/GlobalScheme.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GlobalScheme : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GlobalScheme()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GlobalScheme"",
    ""maps"": [
        {
            ""name"": ""Global"",
            ""id"": ""f0453d7e-36f2-42af-bf31-84b44ba2a139"",
            ""actions"": [
                {
                    ""name"": ""Join"",
                    ""type"": ""Button"",
                    ""id"": ""f1d96226-c2c1-45d7-ba98-b7d37be75107"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""9c491c84-4325-402b-8b93-63e9293c7968"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""94414eab-25ab-4939-887b-9737999af5e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2ea5b038-cc44-450a-92f5-9d7d21d1e6e8"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Join"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39928a4f-58d1-4f93-aa7e-4d7187292b28"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Join"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ccf370c1-db29-4f0c-88b8-d2cb60aaa739"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""885b027a-f09c-4bd0-a019-cfba8b0bf02d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9aa3c1fa-e3f2-4b58-b25a-7307e53d5904"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8adccc2-2017-46cb-bac1-9d5ae4d217de"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Global
        m_Global = asset.FindActionMap("Global", throwIfNotFound: true);
        m_Global_Join = m_Global.FindAction("Join", throwIfNotFound: true);
        m_Global_Pause = m_Global.FindAction("Pause", throwIfNotFound: true);
        m_Global_Cancel = m_Global.FindAction("Cancel", throwIfNotFound: true);
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

    // Global
    private readonly InputActionMap m_Global;
    private IGlobalActions m_GlobalActionsCallbackInterface;
    private readonly InputAction m_Global_Join;
    private readonly InputAction m_Global_Pause;
    private readonly InputAction m_Global_Cancel;
    public struct GlobalActions
    {
        private @GlobalScheme m_Wrapper;
        public GlobalActions(@GlobalScheme wrapper) { m_Wrapper = wrapper; }
        public InputAction @Join => m_Wrapper.m_Global_Join;
        public InputAction @Pause => m_Wrapper.m_Global_Pause;
        public InputAction @Cancel => m_Wrapper.m_Global_Cancel;
        public InputActionMap Get() { return m_Wrapper.m_Global; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlobalActions set) { return set.Get(); }
        public void SetCallbacks(IGlobalActions instance)
        {
            if (m_Wrapper.m_GlobalActionsCallbackInterface != null)
            {
                @Join.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnJoin;
                @Join.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnJoin;
                @Join.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnJoin;
                @Pause.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnPause;
                @Cancel.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnCancel;
            }
            m_Wrapper.m_GlobalActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Join.started += instance.OnJoin;
                @Join.performed += instance.OnJoin;
                @Join.canceled += instance.OnJoin;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
            }
        }
    }
    public GlobalActions @Global => new GlobalActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IGlobalActions
    {
        void OnJoin(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
    }
}
