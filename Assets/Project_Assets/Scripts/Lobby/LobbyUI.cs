using System;
using System.Collections.Generic;
using System.Linq;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries;
using Project_Assets.Scripts.Structs;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        public static Action OnStartGame;
        
        public Unity.Services.Lobbies.Models.Lobby CurrentLobby { get; set; }
        
        [SerializeField] private GameObject m_playerListItemPrefab;
        [SerializeField] private Transform m_playerListItemContainer;
        
        [Header("Lobby Panel Elements")] 
        [SerializeField] public Button StartGameButton;
        [SerializeField] private Button m_leaveLobbyButton;
        [SerializeField] private TMP_Text m_gameCodeText;
        [SerializeField] private ImagesDictionary m_gameImagesDictionary;
        [SerializeField] private LobbyInfo m_lobbyInfo;
        public LobbyInfo LobbyInfo => m_lobbyInfo;
        
        private readonly Dictionary<Unity.Services.Lobbies.Models.Player, string> m_playersReadyValues = new();
        
        private LobbyManager m_lobbyManager;
        private ErrorMessageText m_errorMessage;
        private PanelSwitcher m_panelSwitcher;
        private AvailableGamesUI m_availableGamesUI;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyManager);
            ServiceLocator.ForSceneOf(this).Get(out m_errorMessage);
            ServiceLocator.ForSceneOf(this).Get(out m_panelSwitcher);
            ServiceLocator.ForSceneOf(this).Get(out m_availableGamesUI);
            
            m_lobbyManager.OnCreateLobbyAsync += OnCreateLobbyAsync;
            m_lobbyManager.OnSetGameCode += OnSetGameCode;
            m_lobbyManager.OnSettingsUpdate += OnUpdateLobbyInfo;
            m_lobbyManager.OnLobbyPlayerUpdate += OnLobbyPlayerUpdate;
            m_lobbyManager.OnPlayerJoinedLobbyAsync += OnPlayerJoinedLobbyAsync;
            m_lobbyManager.OnPlayerLeftLobbyAsync += OnPlayerLeftLobbyAsync;
            
            StartGameButton.onClick.AddListener(OnHostStartGame);
            m_leaveLobbyButton.onClick.AddListener(OnLeaveLobby);
            
            StartGameButton.interactable = false;
            m_leaveLobbyButton.interactable = true;
        }
        
        private void OnCreateLobbyAsync(LobbyEventArgs obj)
        {
            m_panelSwitcher.SwitchPanel(LobbyPanel.Lobby);
            CurrentLobby = obj.Lobby;
        }

        private void OnHostStartGame()
        {
            // TODO: Lobby players leave button still active during game starting countdown
            m_leaveLobbyButton.interactable = false;
            OnStartGame?.Invoke();
        }

        private void OnSetGameCode(string obj)
        {
            m_gameCodeText.text = obj;
        }

        private async void OnLeaveLobby()
        {
            var task = await m_lobbyManager.LeaveLobbyAsync();
            PrintStatusLog(task, LobbyPanel.GamePanel);
            m_availableGamesUI.CurrentSelectedLobby = null;
        }

        private void OnLobbyPlayerUpdate(LobbyEventArgs e)
        {
            UpdatePlayerList(e.Lobby);
            Debug.Log("Populate Player List".Color("cyan"));
        }

        private void OnPlayerJoinedLobbyAsync(LobbyEventArgs obj)
        {
            m_panelSwitcher.SwitchPanel(LobbyPanel.Lobby);
            CurrentLobby = obj.Lobby;
            Debug.Log("Player Joined Lobby".Color("cyan"));
        }

        private void OnPlayerLeftLobbyAsync(LobbyEventArgs obj)
        {
            m_panelSwitcher.SwitchPanel(LobbyPanel.GamePanel);
            Debug.Log($"Player Left; Lobby Id: {obj.Lobby.Id}");
            m_availableGamesUI.CurrentSelectedLobby = null;
            m_availableGamesUI.GameNameText.text = string.Empty;
            CurrentLobby = null;
        }

        private void UpdatePlayerList(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            if (lobby?.Players == null)
            {
                Debug.LogWarning("Lobby players is null or empty".Color("red"));
                m_playerListItemContainer.ClearContainer();
                return;
            }

            m_playerListItemContainer.ClearContainer();
            
            RemoveKickedPlayerFromReadyValues(lobby);
            
            foreach (var player in lobby.Players)
            {
                string playerName = player.Data[KeyConstants.k_PlayerName].Value;

                string playerId = player.Data[KeyConstants.k_PlayerId].Value;

                var entry = Instantiate(m_playerListItemPrefab, m_playerListItemContainer).GetComponent<PlayerListItem>();
                
                 entry.Initialize(playerName, playerId, new PlayerConfiguration
                 {
                     Player = player,
                     IsHostPlayer = playerId == lobby.HostId,
                     IsLocalPlayer = playerId == AuthenticationService.Instance.PlayerId,
                 });
                
                 entry.gameObject.SetActive(true);
                
                var localIsHost = AuthenticationService.Instance.PlayerId == lobby.HostId;
                StartGameButton.gameObject.SetActive(localIsHost);
                
                var readyValue = player.Data[KeyConstants.k_PlayerReady].Value;

                if (localIsHost)
                {
                    if (!m_playersReadyValues.TryAdd(player, readyValue))
                    {
                        m_playersReadyValues[player] = readyValue;
                    }
                }
            }

            UpdateStartGameButton();
        }

        private void RemoveKickedPlayerFromReadyValues(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            foreach (var player in m_playersReadyValues.Keys.ToList().Where(player => !lobby.Players.Contains(player)))
            {
                m_playersReadyValues.Remove(player);
            }
        }

        private void UpdateStartGameButton()
        {
            if (m_playersReadyValues.Where(playerReady => playerReady.Value != "true").Any(player => player.Value == "false"))
            {
                StartGameButton.interactable = false;
                return;
            }

            StartGameButton.interactable = true;
        }

        private void OnUpdateLobbyInfo(LobbyEventArgs lobbyEventArgs)
        {
            Debug.Log("On Update Lobby Info".Color("cyan"));

            m_lobbyInfo.GameName.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameName].Value;
            m_lobbyInfo.MaxPlayers.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_MaxPlayers].Value;
            m_lobbyInfo.GameSpeed.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameSpeed].Value;
            m_lobbyInfo.GameMode.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameMode].Value;
            m_lobbyInfo.MapName.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_Map].Value;

            m_lobbyInfo.GameImage.color = Color.white;

            if (lobbyEventArgs.Lobby.Data[KeyConstants.k_GameImage].Value != null)
            {
                m_lobbyInfo.GameImage.texture =
                    m_gameImagesDictionary[lobbyEventArgs.Lobby.Data[KeyConstants.k_GameImage].Value];
            }
        }

        private void PrintStatusLog(StatusReport task, LobbyPanel panel)
        {
            if (!task.Success)
            {
                m_errorMessage.ShowError(task.Message, panel);
                return;
            }

            task.Log();
        }
    }
}