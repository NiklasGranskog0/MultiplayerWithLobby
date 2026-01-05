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
        public event Action OnMouseClickEvent;
        
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
        
        public void OnMousePosition(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnMouseMovedEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMouseClick(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnMouseClickEvent?.Invoke();
        }

        public void OnMouseAxis(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnMouseAxisEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }
}
