using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class GameListItem : MonoBehaviour
    {
        public TMP_Text gameName;
        public TMP_Text gameSpeed;
        public TMP_Text gameMode;
        public TMP_Text maxPlayers;
        public TMP_Text playerCount; // show how many players in lobby/game
        public Button joinButton;
        
        private Unity.Services.Lobbies.Models.Lobby m_Lobby;
        
        private LobbyManager m_LobbyManager;
        
        private void Awake()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            
            joinButton.onClick.AddListener(JoinThisGame);
        }

        // TEMP
        private async void JoinThisGame()
        {
            var report = await m_LobbyManager.JoinLobbyByIdAsync(m_Lobby.Id);
            report.Log();
        }

        public void UpdateLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            m_Lobby = lobby;

            gameName.text = lobby.Data[KeyConstants.k_GameName].Value;
            maxPlayers.text = lobby.Data[KeyConstants.k_MaxPlayers].Value;
            gameMode.text = lobby.Data[KeyConstants.k_GameMode].Value;
            gameSpeed.text = lobby.Data[KeyConstants.k_GameSpeed].Value;
            
            // TODO: Set info in side panel
        }
    }
}
