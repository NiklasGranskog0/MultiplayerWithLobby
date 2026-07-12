using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Game.UI;
using Project_Assets.Scripts.Interfaces;
using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_Assets.Scripts.Player
{
    public class PlayerCharacterBase : NetworkBehaviour, ISelectionObject
    {
        [SerializeField] private PlayerInputs m_playerInputsComponent;
        [SerializeField] private PlayerAnimations m_playerAnimationsComponent;
        [SerializeField] private PlayerMovement m_playerMovementComponent;
        [SerializeField] private Transform m_cameraStartPosition;
        [SerializeField] private ObjectTargeter m_objectTargeterComponent;
        [SerializeField] private PlayerMenuButtons m_playerMenuButtons;
        private PlayerCamera m_playerCameraComponent;
        private ulong m_playerId;

        public ImageToLoad ImageToLoad => ImageToLoad.Player;
        public string Name => "Player";
        public void SetGameMenuButtons()
        {
            
        }

        private void Start()
        {
            // Player object spawned in the Startup scene, so move it to the Game scene
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));

            var playerCamera = FindObjectsByType<PlayerCamera>();
            foreach (var cam in playerCamera)
            {
                if (cam.PlayerId != m_playerId) continue;

                m_playerCameraComponent = cam;
            }

            m_playerCameraComponent.Initialize(m_playerInputsComponent, m_cameraStartPosition, IsOwner);
            m_playerMovementComponent.Initialize(m_playerInputsComponent, m_playerCameraComponent);
            m_objectTargeterComponent.Initialize(m_playerInputsComponent, m_playerCameraComponent, this,
                gameObject.tag);
            m_playerAnimationsComponent.Initialize(m_playerMovementComponent);
            m_playerMenuButtons.Initialize(m_playerInputsComponent);
        }

        private void Update()
        {
            // Only update if the player is the owner
            if (!IsOwner) return;

            // TODO: If the text chat window is open, don't update player movement
            if (m_playerCameraComponent)
            {
                m_playerCameraComponent.OnUpdate(transform.position);
            }

            if (m_playerAnimationsComponent)
            {
                m_playerAnimationsComponent.OnUpdate();
            }
        }

        public override void OnNetworkSpawn() => m_playerId = OwnerClientId;
    }
}