using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_Assets.Scripts.Player
{
    // TODO: Do the player actually need to be a NetworkBehaviour?
    // TODO: Rename to PlayerCharacterBase
    public class PlayerBase : NetworkBehaviour
    {
        [SerializeField] private PlayerInputs m_playerInputsComponent;
        [SerializeField] private PlayerAnimations m_playerAnimationsComponent;
        [SerializeField] private PlayerMovement m_playerMovementComponent;
        [SerializeField] private PlayerCamera m_playerCameraComponent;
        public ulong PlayerId { get; private set; }

        private void Start()
        {
            // Player object spawned in the Startup scene, so move it to the Game scene
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
            
            m_playerCameraComponent.Initialize(m_playerInputsComponent, IsOwner);
            m_playerMovementComponent.Initialize(m_playerInputsComponent);
            
            // Just to see if player object and camera object have the same ID 
            m_playerCameraComponent.PlayerId = PlayerId;
        }

        private void Update()
        {
            // Only update if the player is the owner
            if (!IsOwner) return;

            // If the text chat window is open, don't update player movement
            if (m_playerCameraComponent)
            {
                m_playerCameraComponent.OnUpdate();
            }
        }

        public override void OnNetworkSpawn() => PlayerId = OwnerClientId;
    }
}