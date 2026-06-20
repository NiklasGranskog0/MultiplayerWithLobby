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
        [SerializeField] private Transform m_cameraStartPosition;
        public PlayerCamera PlayerCameraComponent { get; set; }
        private ulong m_playerId { get; set; }

        private void Start()
        {
            // Player object spawned in the Startup scene, so move it to the Game scene
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
            
            var playerCamera = FindObjectsByType<PlayerCamera>();
            foreach (var cam in playerCamera)
            {
                if (cam.PlayerId != m_playerId) continue;

                PlayerCameraComponent = cam;
            }

            PlayerCameraComponent.Initialize(m_playerInputsComponent, m_cameraStartPosition, IsOwner);
            m_playerMovementComponent.Initialize(m_playerInputsComponent, PlayerCameraComponent);
        }

        private void Update()
        {
            // Only update if the player is the owner
            if (!IsOwner) return;

            // If the text chat window is open, don't update player movement
            if (PlayerCameraComponent)
            {
                PlayerCameraComponent.OnUpdate();
            }
        }

        public override void OnNetworkSpawn() => m_playerId = OwnerClientId;
    }
}