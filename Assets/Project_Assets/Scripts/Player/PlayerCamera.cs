using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

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
        private PlayerInputs m_PlayerInputs;
        
        private CameraFollowMode m_CameraFollowMode = CameraFollowMode.Never;

        public void Initialize(PlayerInputs playerInputs, bool isOwner)
        {
            m_PlayerInputs = playerInputs;
            m_PlayerInputs.OnCameraEvent += OnCameraEvent;
            m_PlayerInputs.OnCameraMoveEvent += OnCameraMoveEvent;
            
            playerCamera.gameObject.SetActive(isOwner);
        }

        private void OnCameraMoveEvent()
        {
            Debug.Log("CLicked on left mouse button, move camera");
        }

        private void OnCameraEvent(Vector2 vector2)
        {
            
        }

        public void OnUpdate()
        {
            
        }
    }
}
