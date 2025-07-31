using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class GameListItem : MonoBehaviour
    {
        public TMP_Text gameName;
        public TMP_Text playerCount; // show how many players in lobby/game
        public Button joinButton;
        
        private Unity.Services.Lobbies.Models.Lobby m_Lobby;
        
        private LobbyManager m_LobbyManager;
        
        private void Awake()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            
            joinButton.onClick.AddListener(JoinThisGame);
        }

        private async void JoinThisGame()
        {
            var report = await m_LobbyManager.JoinLobbyByIdAsync(m_Lobby.Id);
            report.Log();
        }

        public void UpdateLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            m_Lobby = lobby;
            
            gameName.text = lobby.Name;
            // TODO: Player count, info
            // Set map info in side panel
        }
    }
}
