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
        // public event Action OnMovementEventEnd;
        
        public event Action<Vector2> OnCameraEvent;
        // public event Action OnCameraEventEnd;
        
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
            
            // if (context.canceled)
            //     OnMovementEventEnd?.Invoke();
        }
        
        public void OnCamera(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnCameraEvent?.Invoke(context.ReadValue<Vector2>());
            
            // if (context.canceled)
            //     OnCameraEventEnd?.Invoke();
        }
    }
}
