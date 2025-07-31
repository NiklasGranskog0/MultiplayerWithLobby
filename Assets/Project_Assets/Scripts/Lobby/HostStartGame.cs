using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class HostStartGame : NetworkBehaviour
    {
        [SerializeField] private Button startGameButton;

        private void Start()
        {
            if (startGameButton) startGameButton = GetComponent<Button>();
            
            if (!IsHost) startGameButton.interactable = false;
        }
    }
}
