using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public struct PlayerConfiguration
    {
        public Unity.Services.Lobbies.Models.Player Player;
        public bool IsHostPlayer;
        public bool IsLocalPlayer;
    }

    public class PlayerListItem : MonoBehaviour
    {
        public TMP_Text PlayerName;
        public Button KickButton;
        public Button ReadyButton;
        public TMP_Text ReadyButtonText;

        public TMP_Dropdown TeamDropdown;
        private PlayerConfiguration m_playerConfiguration;

        private string PlayerId { get; set; }
        private bool PlayerReady { get; set; }

        private LobbyManager m_lobbyManager;
        private ErrorMessageText m_errorMessageText;

        public void Initialize(string pName, string playerId, PlayerConfiguration config)
        {
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyManager);
            ServiceLocator.ForSceneOf(this).Get(out m_errorMessageText);

            PlayerId = playerId;
            m_playerConfiguration = config;

            if (m_playerConfiguration.IsHostPlayer)
            {
                PlayerName.text = pName + " [Host]";
            }
            else PlayerName.text = pName;


            ShowKickButton();
            KickButton.onClick.AddListener(OnClickKickButton);

            TeamDropdown.onValueChanged.AddListener(OnTeamSelectionChanged);
            TeamDropdown.interactable = m_playerConfiguration.IsLocalPlayer;

            ReadyButton.onClick.AddListener(OnReadyClick);
            ReadyButton.interactable = m_playerConfiguration.IsLocalPlayer;

            if (m_playerConfiguration.IsHostPlayer)
            {
                ReadyButton.gameObject.SetActive(false);
                PlayerReady = true;
                m_playerConfiguration.Player.Data[KeyConstants.k_PlayerReady].Value = "true";
            }
           
            // Initialize dropdown from player data without triggering callbacks
            if (m_playerConfiguration.Player.Data != null &&
                m_playerConfiguration.Player.Data.ContainsKey(KeyConstants.k_PlayerTeam) &&
                int.TryParse(m_playerConfiguration.Player.Data[KeyConstants.k_PlayerTeam].Value, out var teamIndex))
            {
                TeamDropdown.SetValueWithoutNotify(teamIndex);
            }
            else
            {
                TeamDropdown.SetValueWithoutNotify(0);
            }

            // Initialize ready state visuals based on player data
            if (!m_playerConfiguration.IsHostPlayer)
            {
                bool ready = false;
                if (m_playerConfiguration.Player.Data != null &&
                    m_playerConfiguration.Player.Data.ContainsKey(KeyConstants.k_PlayerReady) &&
                    bool.TryParse(m_playerConfiguration.Player.Data[KeyConstants.k_PlayerReady].Value,
                        out var parsedReady))
                {
                    ready = parsedReady;
                }

                if (ready)
                {
                    if (ReadyButton.targetGraphic != null) ReadyButton.targetGraphic.color = Color.green;
                    if (ReadyButtonText != null) ReadyButtonText.text = "Ready";
                }
                else
                {
                    if (ReadyButton.targetGraphic != null) ReadyButton.targetGraphic.color = Color.red;
                    if (ReadyButtonText != null) ReadyButtonText.text = "Not Ready";
                }

                PlayerReady = ready;
            }
        }

        private async void OnReadyClick()
        {
            PlayerReady = !PlayerReady;

            var report = await m_lobbyManager.UpdateReadyButton(PlayerId, PlayerReady);

            if (!report.Success)
            {
                // Revert on failure
                m_errorMessageText.ShowError(report.Message, LobbyPanel.Lobby);

                PlayerReady = !PlayerReady;

                if (PlayerReady)
                {
                    if (ReadyButton.targetGraphic != null) ReadyButton.targetGraphic.color = Color.green;
                    if (ReadyButtonText != null) ReadyButtonText.text = "Ready";
                }
                else
                {
                    if (ReadyButton.targetGraphic != null) ReadyButton.targetGraphic.color = Color.red;
                    if (ReadyButtonText != null) ReadyButtonText.text = "Not Ready";
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
            if (m_lobbyManager != null && m_lobbyManager.ActiveLobby != null)
            {
                localIsHost = AuthenticationService.Instance.PlayerId == m_lobbyManager.ActiveLobby.HostId;
            }

            if (!localIsHost)
            {
                KickButton.gameObject.SetActive(false);
                return;
            }

            // Local host shouldn't see a kick button on themselves
            KickButton.gameObject.SetActive(!m_playerConfiguration.IsLocalPlayer);
        }

        private async void OnTeamSelectionChanged(int index)
        {
            var report = await m_lobbyManager.UpdatePlayerTeamAsync(PlayerId, index);

            if (!report.Success) m_errorMessageText.ShowError(report.Message, LobbyPanel.Lobby);
            else report.Log();
        }

        private async void OnClickKickButton()
        {
            var report = await m_lobbyManager.KickPlayerAsync(PlayerId);
            if (!report.Success) m_errorMessageText.ShowError(report.Message, LobbyPanel.Lobby);
            else report.Log();
        }

        public void Reset()
        {
            KickButton.onClick.RemoveListener(OnClickKickButton);
            TeamDropdown.onValueChanged.RemoveListener(OnTeamSelectionChanged);
            PlayerId = null;
        }
    }
}