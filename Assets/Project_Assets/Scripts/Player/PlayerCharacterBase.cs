using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Game.UI;
using Project_Assets.Scripts.Interfaces;
using Project_Assets.Scripts.ScriptableObjects;
using Project_Assets.Scripts.Structs;
using Project_Assets.Scripts.UtilityExtensions.GameObjects;
using Project_Assets.Scripts.UtilityExtensions.Strings;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Project_Assets.Scripts.Player
{
    // TODO: Disable player controls until the game scene is shown

    public class PlayerCharacterBase : NetworkBehaviour, ISelectionObject
    {
        [SerializeField] private PlayerPrefabs m_playerPrefabs;
        [SerializeField] private PlayerInputs m_playerInputsComponent;
        [SerializeField] private PlayerAnimations m_playerAnimationsComponent;
        [SerializeField] private PlayerMovement m_playerMovementComponent;
        [SerializeField] private Transform m_cameraStartPosition;
        [SerializeField] private PlayerMenuButtons m_playerMenuButtons;
        private ObjectTargeter m_objectTargeterComponent;
        private PlayerCamera m_playerCameraComponent;

        public NetworkVariable<Team> PlayerTeam = new();
        private void ApplyTeamTag(Team team) => gameObject.tag = team.ToString();
        private void OnTeamChanged(Team oldValue, Team newValue) => ApplyTeamTag(newValue);
        
        public ulong PlayerId { get; private set; }

        // UI object display
        public ImageToLoad ImageToLoad => ImageToLoad.Player;
        public string Name => "Player";

        public void SetGameMenuButtons() => m_playerMenuButtons.SetGameMenuButtons();

        // Can move from start function to after on network spawn
        private void Start()
        {
            // Moving the Player object to the Game scene
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));

            if (!IsOwner) return;

            m_playerMenuButtons.Initialize();
            m_playerCameraComponent.Initialize(m_playerInputsComponent, m_cameraStartPosition);
            m_playerMovementComponent.Initialize(m_playerInputsComponent, m_playerCameraComponent);
            m_objectTargeterComponent.Initialize(m_playerInputsComponent, m_playerCameraComponent, this, 
                PlayerTeam.Value);
            m_playerAnimationsComponent.Initialize(m_playerMovementComponent);
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

        public override void OnNetworkSpawn()
        {
            ApplyTeamTag(PlayerTeam.Value); // TODO:  Do we need this ? 
            PlayerTeam.OnValueChanged += OnTeamChanged;

            if (!IsOwner) return;
            
            PlayerId = OwnerClientId;

            var cameraObject = Instantiate(m_playerPrefabs.CameraPrefab, m_cameraStartPosition.position,
                Quaternion.identity).Get<PlayerCamera>();

            var objectTargeter =
                Instantiate(m_playerPrefabs.TargeterPrefab, m_cameraStartPosition.position,
                        Quaternion.identity).Get<ObjectTargeter>();

            m_playerCameraComponent = cameraObject;
            m_objectTargeterComponent = objectTargeter;
        }
    }
}