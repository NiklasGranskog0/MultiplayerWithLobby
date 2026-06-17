using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private Transform m_cameraTarget;
        [SerializeField] private float m_cameraMoveSpeed = 15f;
        public ulong PlayerId;

        private Vector3 m_cameraMoveDirection;
        private Vector3 m_mousePosition;
        private PlayerInputs m_playerInputs;
        
        public void Initialize(PlayerInputs playerInputs, bool isOwner)
        {
            m_playerInputs = playerInputs;
            m_playerInputs.OnMovementEvent += SetCameraMoveDirection;
            
            m_playerCamera.gameObject.SetActive(isOwner);
            transform.rotation = Quaternion.Euler(-15f, -90f, 0);
            
            transform.SetParent(null, true);
        }

        public void OnUpdate()
        {
            m_cameraTarget.position += m_cameraMoveDirection.normalized * (m_cameraMoveSpeed * Time.deltaTime);
        }
        
        // Moves camera with WASD keys
        private void SetCameraMoveDirection(Vector2 direction)
        {
            m_cameraMoveDirection.x = -direction.y;
            m_cameraMoveDirection.z = direction.x;
            // m_playerInputs.InvokeMouseMoveEvent();
        }
    }
}
