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
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""70bf235d-0333-42b1-8494-087cd5300d0a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Join"",
                    ""type"": ""Button"",
                    ""id"": ""f1d96226-c2c1-45d7-ba98-b7d37be75107"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""36d1c60e-4671-48e4-85ff-ce6d8320213b"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ea5b038-cc44-450a-92f5-9d7d21d1e6e8"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
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
                    ""groups"": """",
                    ""action"": ""Join"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Global
        m_Global = asset.FindActionMap("Global", throwIfNotFound: true);
        m_Global_Move = m_Global.FindAction("Move", throwIfNotFound: true);
        m_Global_Join = m_Global.FindAction("Join", throwIfNotFound: true);
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
    private readonly InputAction m_Global_Move;
    private readonly InputAction m_Global_Join;
    public struct GlobalActions
    {
        private @GlobalScheme m_Wrapper;
        public GlobalActions(@GlobalScheme wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Global_Move;
        public InputAction @Join => m_Wrapper.m_Global_Join;
        public InputActionMap Get() { return m_Wrapper.m_Global; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlobalActions set) { return set.Get(); }
        public void SetCallbacks(IGlobalActions instance)
        {
            if (m_Wrapper.m_GlobalActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnMove;
                @Join.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnJoin;
                @Join.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnJoin;
                @Join.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnJoin;
            }
            m_Wrapper.m_GlobalActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Join.started += instance.OnJoin;
                @Join.performed += instance.OnJoin;
                @Join.canceled += instance.OnJoin;
            }
        }
    }
    public GlobalActions @Global => new GlobalActions(this);
    public interface IGlobalActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJoin(InputAction.CallbackContext context);
    }
}
