using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Assets.Scripts.Player
{
    public enum CameraFollowMode
    {
        Never,
    }
    
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float rotationSpeed = 10f;
        
        // [SerializeField] private float minPitch = 5f; slightly above ground
        // [SerializeField] private float maxPitch = 80f; not straight overhead
        
        private PlayerInputs m_PlayerInputs;

        public CameraFollowMode FollowMode { get; set; }

        private Vector2 m_MousePosition;
        private Vector2 m_MouseDelta;
        
        public void Initialize(PlayerInputs playerInputs, bool isOwner)
        {
            m_PlayerInputs = playerInputs;
            m_PlayerInputs.OnMouseMovedEvent += OnMouseMoved;
            m_PlayerInputs.OnMouseClickEvent += OnMouseClickEvent;
            m_PlayerInputs.OnMouseAxisEvent += OnMouseAxisEvent;
            
            playerCamera.gameObject.SetActive(isOwner);
        }

        private void OnMouseClickEvent()
        {
            
        }

        private void OnMouseAxisEvent(Vector2 vector2)
        {
            m_MouseDelta = vector2;
        }

        private void OnMouseMoved(Vector2 vector2)
        {
            m_MousePosition = vector2.normalized;
        }
        
        public void OnUpdate()
        {
            if (playerCamera != null && cameraTarget != null)
            {
                playerCamera.transform.LookAt(cameraTarget.position);
            }

            // Rotate camera around target
            if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            {
                var dy = m_MouseDelta.y * rotationSpeed * Time.deltaTime;
                var dx = m_MouseDelta.x * rotationSpeed * Time.deltaTime;
                
                cameraTarget.Rotate(Vector3.right, dy);
                cameraTarget.Rotate(Vector3.up, dx, Space.World);
            }
        }
    }
}
