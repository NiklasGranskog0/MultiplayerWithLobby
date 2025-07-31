using Project_Assets.Scripts.Enums;
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
        public PlayerConfiguration PlayerConfiguration;
        public Team team;

        public string PlayerId { get; set; }

        private LobbyManager m_LobbyManager;

        public void Initialize(string pName, string playerId, PlayerConfiguration config)
        {
            playerName.text = pName;
            PlayerId = playerId;
            PlayerConfiguration = config;

            // config.Player.Data["PlayerId"].Value = PlayerId;
            
            ShowKickButton();
            kickButton.onClick.AddListener(OnClickKickButton);

            ServiceLocator.Global.Get<LobbyManager>(out var manager);
            m_LobbyManager = manager;
        }

        // If host has been changed
        public void OnHostChanged()
        {
            ShowKickButton();
        }

        private void ShowKickButton()
        {
            kickButton.gameObject.SetActive(!PlayerConfiguration.IsHostPlayer && !PlayerConfiguration.IsLocalPlayer);
        }

        private async void OnClickKickButton()
        {
            var report = await m_LobbyManager.KickPlayerAsync(PlayerId);
            report.Log();
        }

        public void Reset()
        {
            kickButton.onClick.RemoveListener(OnClickKickButton);

            PlayerId = null;
        }
    }
}
