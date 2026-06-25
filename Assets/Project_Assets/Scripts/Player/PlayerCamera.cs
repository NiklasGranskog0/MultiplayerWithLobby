using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Project_Assets.Scripts.Player
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private Transform m_cameraTarget;
        [SerializeField] private float m_cameraMoveSpeed = 15f;
        private PlayerInputs m_playerInputs;
        public ulong PlayerId;

        private Vector3 m_cameraMoveDirection;
        private Vector3 m_mousePosition;
        private Vector3 m_playerPosition;
        
        public Ray MouseRay 
        {
            get
            {
                if (Mouse.current != null)
                {
                    m_mousePosition = Mouse.current.position.ReadValue();
                }
                
                return m_playerCamera.ScreenPointToRay(m_mousePosition);
            }
        }

        private void Start()
        {
            // Camera object spawned in the Startup scene, so move it to the Game scene
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
        }

        public void Initialize(PlayerInputs playerInputs, Transform startTransform, bool isOwner)
        {
            m_playerInputs = playerInputs;
            m_playerInputs.OnMovementEvent += SetCameraMoveDirection;
            m_playerInputs.OnCameraResetEvent += ResetCameraPosition;
            
            m_playerCamera.gameObject.SetActive(isOwner);
            transform.rotation = Quaternion.Euler(-15f, -90f, 0);
            transform.position = startTransform.position;
        }

        public void OnUpdate(Vector3 playerPosition)
        {
            m_playerPosition = playerPosition;
            m_cameraTarget.position += m_cameraMoveDirection.normalized * (m_cameraMoveSpeed * Time.deltaTime);
        }
        
        // Moves camera with WASD keys
        private void SetCameraMoveDirection(Vector2 direction)
        {
            m_cameraMoveDirection.x = -direction.y;
            m_cameraMoveDirection.z = direction.x;
        }

        private void ResetCameraPosition()
        {
            m_cameraTarget.position = new Vector3(m_playerPosition.x + 7.8f, m_cameraTarget.position.y, m_playerPosition.z);
        }

        public override void OnNetworkSpawn() => PlayerId = OwnerClientId;
    }
}
