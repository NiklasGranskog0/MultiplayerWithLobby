using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class GameListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_gameListName;
        [SerializeField] private ImagesDictionary m_imagesDictionary;
        public Button JoinButton;

        private Unity.Services.Lobbies.Models.Lobby m_lobby;
        private AvailableGamesUI m_availableGamesUI;

        private void Awake()
        {
            JoinButton.onClick.AddListener(OnGameListItemClick);
        }

        private void Start()
        {
            ServiceLocator.Global.Get(out m_availableGamesUI);
        }

        public void UpdateLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            m_lobby = lobby;
            m_gameListName.text = m_lobby.Name;
        }

        private void OnGameListItemClick()
        {
            m_availableGamesUI.CurrentSelectedLobby = m_lobby;
            m_availableGamesUI.GameCodeInputField.text = m_lobby.Name;
            m_availableGamesUI.GameNameText.text = m_lobby.Name;

            m_availableGamesUI.GameInfo.GameName.text = m_lobby.Data[KeyConstants.k_GameName].Value;
            m_availableGamesUI.GameInfo.MaxPlayers.text = m_lobby.Data[KeyConstants.k_MaxPlayers].Value;
            m_availableGamesUI.GameInfo.GameMode.text = m_lobby.Data[KeyConstants.k_GameMode].Value;
            m_availableGamesUI.GameInfo.GameSpeed.text = m_lobby.Data[KeyConstants.k_GameSpeed].Value;
            m_availableGamesUI.GameInfo.MapName.text = m_lobby.Data[KeyConstants.k_Map].Value;

            m_availableGamesUI.GameInfo.GameImage.color = Color.white;

            if (m_lobby.Data[KeyConstants.k_GameImage].Value != null)
            {
                m_availableGamesUI.GameInfo.GameImage.texture =
                    m_imagesDictionary[m_lobby.Data[KeyConstants.k_GameImage].Value];
            }
        }
    }
}