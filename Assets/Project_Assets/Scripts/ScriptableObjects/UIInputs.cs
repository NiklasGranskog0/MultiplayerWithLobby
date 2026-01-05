using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "Scriptable Objects/UI Input Reader")]
    public class UIInputs : ScriptableObject, ProjectInputs.IUIActions
    {
        private ProjectInputs m_actions;

        public event Action OnReturnKeyEvent;
        public event Action<Vector2> OnScrollWheelEvent;

        private void OnEnable()
        {
            m_actions ??= new ProjectInputs();
            m_actions.UI.SetCallbacks(this);
            m_actions.Enable();
        }
        
        private void OnDisable()
        {
            m_actions.Disable();
        }
        
        public void OnReturnKey(InputAction.CallbackContext context)
        {
            if (context.performed) { OnReturnKeyEvent?.Invoke(); }
        }

        public void OnNavigate(InputAction.CallbackContext context) { }
        public void OnSubmit(InputAction.CallbackContext context) { }
        public void OnCancel(InputAction.CallbackContext context) { }
        public void OnPoint(InputAction.CallbackContext context) { }
        public void OnClick(InputAction.CallbackContext context) { }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
                OnScrollWheelEvent?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnMiddleClick(InputAction.CallbackContext context) { }
        public void OnRightClick(InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
        
    }
}