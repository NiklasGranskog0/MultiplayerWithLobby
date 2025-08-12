using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using Unity.Services.Authentication;
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
        public Button readyButton;
        public TMP_Text readyButtonText;
        
        public TMP_Dropdown teamDropdown;
        private PlayerConfiguration m_PlayerConfiguration;

        private string PlayerId { get; set; }
        public bool PlayerReady { get; set; }

        private LobbyManager m_LobbyManager;
        private ErrorMessageText m_ErrorMessageText;

        public void Initialize(string pName, string playerId, PlayerConfiguration config)
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            ServiceLocator.Global.Get(out m_ErrorMessageText);

            PlayerId = playerId;
            m_PlayerConfiguration = config;

            if (m_PlayerConfiguration.IsHostPlayer)
            {
                playerName.text = pName + " [Host]";
            }
            else playerName.text = pName;

            ShowKickButton();
            kickButton.onClick.AddListener(OnClickKickButton);

            teamDropdown.onValueChanged.AddListener(OnTeamSelectionChanged);
            teamDropdown.interactable = m_PlayerConfiguration.IsLocalPlayer;
            
            readyButton.onClick.AddListener(OnReadyClick);
            readyButton.interactable = m_PlayerConfiguration.IsLocalPlayer;

            if (m_PlayerConfiguration.IsHostPlayer)
            {
                readyButton.gameObject.SetActive(false);
                PlayerReady = true;
                m_PlayerConfiguration.Player.Data[KeyConstants.k_PlayerReady].Value = "true";
            }

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

            // Initialize ready state visuals based on player data
            if (!m_PlayerConfiguration.IsHostPlayer)
            {
                bool ready = false;
                if (m_PlayerConfiguration.Player.Data != null &&
                    m_PlayerConfiguration.Player.Data.ContainsKey(KeyConstants.k_PlayerReady) &&
                    bool.TryParse(m_PlayerConfiguration.Player.Data[KeyConstants.k_PlayerReady].Value, out var parsedReady))
                {
                    ready = parsedReady;
                }

                if (ready)
                {
                    if (readyButton.targetGraphic != null) readyButton.targetGraphic.color = Color.green;
                    if (readyButtonText != null) readyButtonText.text = "Ready";
                }
                else
                {
                    if (readyButton.targetGraphic != null) readyButton.targetGraphic.color = Color.red;
                    if (readyButtonText != null) readyButtonText.text = "Not Ready";
                }

                PlayerReady = ready;
            }
        }

        private async void OnReadyClick()
        {
            PlayerReady = !PlayerReady;

            var report = await m_LobbyManager.UpdateReadyButton(PlayerId, PlayerReady);

            if (!report.Success)
            {
                // Revert on failure
                m_ErrorMessageText.ShowError(report.Message, LobbyPanel.Lobby);

                PlayerReady = !PlayerReady;

                if (PlayerReady)
                {
                    if (readyButton.targetGraphic != null) readyButton.targetGraphic.color = Color.green;
                    if (readyButtonText != null) readyButtonText.text = "Ready";
                }
                else
                {
                    if (readyButton.targetGraphic != null) readyButton.targetGraphic.color = Color.red;
                    if (readyButtonText != null) readyButtonText.text = "Not Ready";
                }
            }
            else
            {
                report.Log();
            }
        }

        private void ShowKickButton()
        {
            // Only the local host should see kick buttons for other players
            bool localIsHost = false;
            if (m_LobbyManager != null && m_LobbyManager.ActiveLobby != null)
            {
                localIsHost = AuthenticationService.Instance.PlayerId == m_LobbyManager.ActiveLobby.HostId;
            }

            if (!localIsHost)
            {
                kickButton.gameObject.SetActive(false);
                return;
            }

            // Local host shouldn't see a kick button on themselves
            kickButton.gameObject.SetActive(!m_PlayerConfiguration.IsLocalPlayer);
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