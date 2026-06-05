using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerBase : NetworkBehaviour
    {
        [SerializeField] private PlayerInputs m_playerInputsComponent;
        [SerializeField] private PlayerAnimations m_playerAnimationsComponent;
        [SerializeField] private PlayerMovement m_playerMovementComponent;
        [SerializeField] private PlayerCamera m_playerCameraComponent;
        public ulong PlayerId;

        private void Start()
        {
            m_playerCameraComponent.Initialize(m_playerInputsComponent, IsOwner);
            m_playerMovementComponent.Initialize(m_playerInputsComponent);

            // Just to see if player object and camera object are linked 
            PlayerId = m_playerCameraComponent.PlayerId = NetworkObjectId;
        }

        private void Update()
        {
            if (!IsOwner) return;

            // If the text chat window is open, don't update player movement
            if (m_playerCameraComponent)
            {
                m_playerCameraComponent.OnUpdate();
            }
        }
    }
}