using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public struct PlayerConfiguration
    {
        public Player Player;
        public bool IsHostPlayer;
        public bool IsLocalPlayer;
    }

    public class PlayerListItem : MonoBehaviour
    {
        public TMP_Text playerName;
        public Button kickButton;
        public TMP_Dropdown teamDropdown;
        private PlayerConfiguration m_PlayerConfiguration;

        private string PlayerId { get; set; }
        
        private LobbyManager m_LobbyManager;
        private ErrorMessageText m_ErrorMessageText;

        public void Initialize(string pName, string playerId, PlayerConfiguration config)
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            ServiceLocator.Global.Get(out m_ErrorMessageText);

            playerName.text = pName;
            PlayerId = playerId;
            m_PlayerConfiguration = config;

            ShowKickButton();
            kickButton.onClick.AddListener(OnClickKickButton);
            
            teamDropdown.onValueChanged.AddListener(OnTeamSelectionChanged);
            teamDropdown.interactable = m_PlayerConfiguration.IsLocalPlayer;
            
            // Initialize dropdown from player data without triggering callbacks
            if (m_PlayerConfiguration.Player.Data != null &&
                m_PlayerConfiguration.Player.Data.ContainsKey(KeyConstants.k_PlayerTeam) &&
                int.TryParse(m_PlayerConfiguration.Player.Data[KeyConstants.k_PlayerTeam].Value, out var teamIndex))
            {
                teamDropdown.SetValueWithoutNotify(teamIndex);
            }
            else
            {
                teamDropdown.SetValueWithoutNotify(0);
            }
        }

        private void ShowKickButton()
        {
            if (m_PlayerConfiguration is { IsHostPlayer: true })
            {
                kickButton.gameObject.SetActive(false);
            }
            else
            {
                kickButton.gameObject.SetActive(true);
            }
        }

        private async void OnTeamSelectionChanged(int index)
        {
            var report = await m_LobbyManager.UpdatePlayerTeamAsync(PlayerId, index);
            
            if (!report.Success) m_ErrorMessageText.ShowError(report.Message, LobbyPanel.Lobby);
            else report.Log();
        }

        private async void OnClickKickButton()
        {
            var report = await m_LobbyManager.KickPlayerAsync(PlayerId);
            if (!report.Success) m_ErrorMessageText.ShowError(report.Message, LobbyPanel.Lobby);
            else report.Log();
        }

        public void Reset()
        {
            kickButton.onClick.RemoveListener(OnClickKickButton);
            teamDropdown.onValueChanged.RemoveListener(OnTeamSelectionChanged);
            PlayerId = null;
        }
    }
}