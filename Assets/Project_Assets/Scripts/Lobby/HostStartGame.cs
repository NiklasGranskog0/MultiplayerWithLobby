using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class HostStartGame : NetworkBehaviour
    {
        [SerializeField] private Button m_startGameButton;

        private void Start()
        {
            if (m_startGameButton) m_startGameButton = GetComponent<Button>();
            
            if (!IsHost) m_startGameButton.interactable = false;
        }
    }
}
