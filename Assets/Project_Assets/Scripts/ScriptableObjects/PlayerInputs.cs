using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "Scriptable Objects/Input Reader")]
    public class PlayerInputs : ScriptableObject, ProjectInputs.IPlayerActions
    {
        private ProjectInputs m_Actions;
        
        public event Action<Vector2> OnMovementEvent;
        public event Action<Vector2> OnMouseAxisEvent;
        public event Action<Vector2> OnMouseMovedEvent;
        public event Action OnMouseClickEvent;
        
        private void OnEnable()
        {
            m_Actions ??= new();
            m_Actions.Player.SetCallbacks(this);
            m_Actions.Enable();
        }
        
        private void OnDisable()
        {
            m_Actions.Disable();
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
