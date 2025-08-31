using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerBase : NetworkBehaviour
    {
        [SerializeField] private PlayerInputs playerInputs;
        [SerializeField] private PlayerAnimations playerAnimations;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerCamera playerCamera;

        
        private void Awake()
        {
            playerAnimations.Initialize();
            playerMovement.Initialize(playerInputs);
        }

        private void Start()
        {
            playerCamera.Initialize(playerInputs, IsOwner);
        }

        private void Update()
        {
            if (!IsOwner) return;

            playerMovement.OnUpdate();
            playerCamera.OnUpdate();
            playerAnimations.OnUpdate(playerMovement.Speed, playerMovement.Velocity);
        }
    }
}