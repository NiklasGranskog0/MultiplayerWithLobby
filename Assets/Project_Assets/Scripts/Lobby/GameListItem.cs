using System.Text;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class GameListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text gameListName;
        public Button joinButton;

        private Unity.Services.Lobbies.Models.Lobby m_Lobby;
        private LobbyManager m_LobbyManager;
        private LobbyUI m_LobbyUI;

        private void Awake()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            ServiceLocator.Global.Get(out m_LobbyUI);

            joinButton.onClick.AddListener(OnGameListItemClick);
        }

        public void UpdateLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            m_Lobby = lobby;
            gameListName.text = m_Lobby.Name;
        }

        private void OnGameListItemClick()
        {
            m_LobbyUI.CurrentSelectedLobby = m_Lobby;
            m_LobbyUI.gameCodeInputField.text = m_Lobby.Name;
            
            m_LobbyUI.lobbyInfoGames.gameName.text = m_Lobby.Data[KeyConstants.k_GameName].Value;
            m_LobbyUI.lobbyInfoGames.maxPlayers.text = m_Lobby.Data[KeyConstants.k_MaxPlayers].Value;
            m_LobbyUI.lobbyInfoGames.gameMode.text = m_Lobby.Data[KeyConstants.k_GameMode].Value;
            m_LobbyUI.lobbyInfoGames.gameSpeed.text = m_Lobby.Data[KeyConstants.k_GameSpeed].Value;
            m_LobbyUI.lobbyInfoGames.mapName.text = m_Lobby.Data[KeyConstants.k_Map].Value;
            
            m_LobbyUI.lobbyInfoGames.gameImage.color = Color.white;
            
            if (m_Lobby.Data[KeyConstants.k_GameImage].Value != null)
            {
                m_LobbyUI.lobbyInfoGames.gameImage.texture =
                    m_LobbyUI.gameImagesDictionary[m_Lobby.Data[KeyConstants.k_GameImage].Value];
            }
        }
    }
}