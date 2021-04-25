// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace LD48.Input
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Game"",
            ""id"": ""d9ceb2a6-49ab-4e13-9316-6346c7dcc175"",
            ""actions"": [
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""cba3786b-9499-41a4-8047-13fd7c215721"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""1c854f28-942e-435a-aeec-ed137d5b3eb4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""7b877924-fa17-458f-b473-fef734b2d1e9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""41c5c43b-5ef6-47eb-b43f-cf2ce712b935"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""8d4fb139-565d-4013-88c8-074ee2c78e40"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=10)"",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6281a67e-b9f3-437b-90ce-dbad1de215d3"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""de9d3df2-4982-4f94-9c35-21739bea9dde"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""21cd66f4-4771-45b1-9ffe-3f0ab2ab7942"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77b26909-f834-4e84-98c1-87a98369c9b7"",
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
                    ""id"": ""4658c495-8cf1-4603-847c-f6dc553f0c48"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""703d1485-5bd6-4da5-a207-05cf44d63e3e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Game
            m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
            m_Game_Scroll = m_Game.FindAction("Scroll", throwIfNotFound: true);
            m_Game_MousePosition = m_Game.FindAction("MousePosition", throwIfNotFound: true);
            m_Game_Cancel = m_Game.FindAction("Cancel", throwIfNotFound: true);
            m_Game_Interact = m_Game.FindAction("Interact", throwIfNotFound: true);
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

        // Game
        private readonly InputActionMap m_Game;
        private IGameActions m_GameActionsCallbackInterface;
        private readonly InputAction m_Game_Scroll;
        private readonly InputAction m_Game_MousePosition;
        private readonly InputAction m_Game_Cancel;
        private readonly InputAction m_Game_Interact;
        public struct GameActions
        {
            private @Controls m_Wrapper;
            public GameActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Scroll => m_Wrapper.m_Game_Scroll;
            public InputAction @MousePosition => m_Wrapper.m_Game_MousePosition;
            public InputAction @Cancel => m_Wrapper.m_Game_Cancel;
            public InputAction @Interact => m_Wrapper.m_Game_Interact;
            public InputActionMap Get() { return m_Wrapper.m_Game; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
            public void SetCallbacks(IGameActions instance)
            {
                if (m_Wrapper.m_GameActionsCallbackInterface != null)
                {
                    @Scroll.started -= m_Wrapper.m_GameActionsCallbackInterface.OnScroll;
                    @Scroll.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnScroll;
                    @Scroll.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnScroll;
                    @MousePosition.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                    @MousePosition.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                    @MousePosition.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                    @Cancel.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCancel;
                    @Cancel.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCancel;
                    @Cancel.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCancel;
                    @Interact.started -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                }
                m_Wrapper.m_GameActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Scroll.started += instance.OnScroll;
                    @Scroll.performed += instance.OnScroll;
                    @Scroll.canceled += instance.OnScroll;
                    @MousePosition.started += instance.OnMousePosition;
                    @MousePosition.performed += instance.OnMousePosition;
                    @MousePosition.canceled += instance.OnMousePosition;
                    @Cancel.started += instance.OnCancel;
                    @Cancel.performed += instance.OnCancel;
                    @Cancel.canceled += instance.OnCancel;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                }
            }
        }
        public GameActions @Game => new GameActions(this);
        public interface IGameActions
        {
            void OnScroll(InputAction.CallbackContext context);
            void OnMousePosition(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
        }
    }
}
