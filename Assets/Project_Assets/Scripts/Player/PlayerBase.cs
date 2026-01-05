using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerBase : NetworkBehaviour
    {
        [SerializeField] private PlayerInputs m_playerInputs;
        [SerializeField] private PlayerAnimations m_playerAnimations;
        [SerializeField] private PlayerMovement m_playerMovement;
        [SerializeField] private PlayerCamera m_playerCamera;

        
        private void Awake()
        {
            m_playerAnimations.Initialize();
            m_playerMovement.Initialize(m_playerInputs);
        }

        private void Start()
        {
            m_playerCamera.Initialize(m_playerInputs, IsOwner);
        }

        private void Update()
        {
            if (!IsOwner) return;

            // If the text chat window is open, don't update player movement
            m_playerMovement.OnUpdate();
            m_playerCamera.OnUpdate();
            m_playerAnimations.OnUpdate(m_playerMovement.Speed, m_playerMovement.Velocity);
        }
    }
}