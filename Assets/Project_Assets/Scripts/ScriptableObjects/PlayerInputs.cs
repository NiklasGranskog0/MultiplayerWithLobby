using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "Scriptable Objects/Player Input Reader")]
    public class PlayerInputs : ScriptableObject, ProjectInputs.IPlayerActions
    {
        private ProjectInputs m_actions;
        
        public event Action<Vector2> OnMovementEvent;
        public event Action<Vector2> OnMouseAxisEvent;
        public event Action<Vector2> OnMouseMovedEvent;
        public event Action OnRightMouseClickEvent;
        public event Action OnLeftMouseClickEvent;
        public event Action OnCameraResetEvent;

        private void OnEnable()
        {
            m_actions ??= new ProjectInputs();
            m_actions.Player.SetCallbacks(this);
            m_actions.Enable();
        }
        
        private void OnDisable()
        {
            m_actions.Disable();
        }
        
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnMovementEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void InvokeMouseMoveEvent()
        {
            OnMouseMovedEvent?.Invoke(Mouse.current.position.ReadValue());
        }
        
        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnMouseMovedEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRightMouseClick(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnRightMouseClickEvent?.Invoke();
        }

        public void OnLeftMouseClick(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnLeftMouseClickEvent?.Invoke();
        }

        public void OnMouseAxis(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnMouseAxisEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnCameraReset(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnCameraResetEvent?.Invoke();
        }
    }
}
