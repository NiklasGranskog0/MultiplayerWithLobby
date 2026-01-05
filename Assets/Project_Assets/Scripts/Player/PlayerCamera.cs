using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Assets.Scripts.Player
{
    // TODO: Move to enums folder
    public enum CameraFollowMode
    {
        Never,
    }
    
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private Transform m_cameraTarget;
        [SerializeField] private float m_rotationSpeed = 10f;
        
        // [SerializeField] private float minPitch = 5f; slightly above ground
        // [SerializeField] private float maxPitch = 80f; not straight overhead
        
        private PlayerInputs m_playerInputs;

        public CameraFollowMode FollowMode { get; set; }

        private Vector2 m_mousePosition;
        private Vector2 m_mouseDelta;
        
        public void Initialize(PlayerInputs playerInputs, bool isOwner)
        {
            m_playerInputs = playerInputs;
            m_playerInputs.OnMouseMovedEvent += OnMouseMoved;
            m_playerInputs.OnMouseClickEvent += OnMouseClickEvent;
            m_playerInputs.OnMouseAxisEvent += OnMouseAxisEvent;
            
            m_playerCamera.gameObject.SetActive(isOwner);
        }

        private void OnMouseClickEvent()
        {
            
        }

        private void OnMouseAxisEvent(Vector2 vector2)
        {
            m_mouseDelta = vector2;
        }

        private void OnMouseMoved(Vector2 vector2)
        {
            m_mousePosition = vector2.normalized;
        }
        
        public void OnUpdate()
        {
            if (m_playerCamera != null && m_cameraTarget != null)
            {
                m_playerCamera.transform.LookAt(m_cameraTarget.position);
            }

            // Rotate camera around target
            if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            {
                var dy = m_mouseDelta.y * m_rotationSpeed * Time.deltaTime;
                var dx = m_mouseDelta.x * m_rotationSpeed * Time.deltaTime;
                
                m_cameraTarget.Rotate(Vector3.right, dy);
                m_cameraTarget.Rotate(Vector3.up, dx, Space.World);
            }
        }
    }
}
