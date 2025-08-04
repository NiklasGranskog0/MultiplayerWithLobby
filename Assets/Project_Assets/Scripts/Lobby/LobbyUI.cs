using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject gamesListPanel;
        [SerializeField] private GameObject createGamePanel;
        [SerializeField] private GameObject lobbyPanel;

        [Header("List Items")] 
        [SerializeField] private GameObject lobbyEntryPrefab;
        [SerializeField] private Transform lobbyListContainer;

        [Space]
        
        [SerializeField] private GameObject playerEntryPrefab;
        [SerializeField] private Transform playerListContainer;
        
        [Space]
        
        [SerializeField] private GameObject chatListItem;
        [SerializeField] private Transform chatContainer;
        
        [Space]
        
        [Header("Create Game Panel Elements")]
        [SerializeField] private TMP_InputField gameNameInputField;
        [SerializeField] private TMP_Dropdown gameModeDropdown;
        [SerializeField] private TMP_Dropdown maxPlayersDropdown;
        [SerializeField] private TMP_Dropdown visibilityDropdown;
        [SerializeField] private TMP_Dropdown gameSpeedDropdown;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button cancelCreateButton;

        [Header("Games Panel Elements")]
        [SerializeField] private TMP_InputField gameCodeInputField;
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button joinGameButton;
        [SerializeField] private Button refreshGamesButton;

        [Header("Lobby Panel Elements")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private Image gameImage;
        [SerializeField] private TMP_Text gameName;
        [SerializeField] private TMP_Text gameMode;
        [SerializeField] private TMP_Text gameSpeed;
        [SerializeField] private TMP_Text maxPlayers;
        
        private int MaxPlayers => maxPlayersDropdown.value + 1;
        private int GameSpeedIndex => gameSpeedDropdown.value;
        private int GameModeIndex => gameModeDropdown.value;
        private string GameName => gameNameInputField.text;

        private LobbyManager m_LobbyManager;
        
        private void Start()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);

            // Value == index
            maxPlayersDropdown.value = 3; // (Set default value to 4 players)
            
            createGameButton.onClick.AddListener(OnCreateLobbySettings);
            cancelCreateButton.onClick.AddListener(OnCancelCreateLobby);
            refreshGamesButton.onClick.AddListener(OnRefreshLobbies);
            createLobbyButton.onClick.AddListener(OnCreateLobby);
            leaveLobbyButton.onClick.AddListener(OnLeaveLobby);

            // joinGameButton.onClick.AddListener(); // Join game by Id / Code

            m_LobbyManager.OnPlayerJoinedLobbyAsync += OnPlayerJoinedLobbyAsync;
            m_LobbyManager.OnPlayerLeftLobbyAsync += OnPlayerLeftLobbyAsync;
            m_LobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;
            m_LobbyManager.OnLobbyListChanged += OnLobbyListChanged;
            m_LobbyManager.OnCreateLobbyAsync += OnCreateLobbyAsync;
            m_LobbyManager.OnSettingsUpdate += OnUpdateLobbyInfo;

            OnRefreshLobbies();
        }

        private async void OnCreateLobby()
        {
            var settings = new CreateLobbySettings
            {
                IsLocked = false,
                IsPrivate = visibilityDropdown.value == 1,
                
                GameMode = ((GameMode)GameModeIndex, DataObject.VisibilityOptions.Public),
                Map = (Map.Forest, DataObject.VisibilityOptions.Public),
                MaxPlayers = (MaxPlayers, DataObject.VisibilityOptions.Public),
                GameName = (GameName, DataObject.VisibilityOptions.Public),
                GameSpeed = ((GameSpeed)GameSpeedIndex, DataObject.VisibilityOptions.Public),
            };
            
            settings.SetData();

            gameName.text = settings.GameName.name;
            maxPlayers.text = settings.MaxPlayers.max.ToString();
            gameSpeed.text = settings.GameSpeed.speed.GameSpeedToString();
            gameMode.text = settings.GameMode.mode.GameModeToString();

            var report = await m_LobbyManager.CreateLobbyAsync(settings);
            report.Log();
        }

        // Temp
        private async void OnRefreshLobbies()
        {
            var lobbiesStatusReport = await m_LobbyManager.GetAllActiveLobbiesAsync();
            PopulateLobbyList(lobbiesStatusReport.Lobbies);
            lobbiesStatusReport.Log();
        }

        private async void OnLeaveLobby()
        {
            var report = await m_LobbyManager.LeaveLobbyAsync();
            report.Log();
        }

        private void OnLobbyListChanged(LobbyListChangedEventArgs e)
        {
            PopulateLobbyList(e.Lobbies);
            Debug.Log("Populate Lobby List".Color("cyan"));
        }

        private void OnJoinedLobbyUpdate(LobbyEventArgs e)
        {
            PopulatePlayerList(e.Lobby);
            Debug.Log("Populate Player List".Color("cyan"));
        }

        private void OnPlayerJoinedLobbyAsync(LobbyEventArgs obj)
        {
            SwitchPanel(LobbyPanel.Lobby);
            Debug.Log("Player Joined Lobby".Color("cyan"));
        }

        private void OnCreateLobbyAsync(LobbyEventArgs obj)
        {
            SwitchPanel(LobbyPanel.Lobby);
        }

        private void OnPlayerLeftLobbyAsync(LobbyEventArgs obj)
        {
            SwitchPanel(LobbyPanel.Games);
            Debug.Log($"Player Left; Lobby Id: {obj.Lobby.Id}");
        }
        
        private void OnCreateLobbySettings()
        {
            SwitchPanel(LobbyPanel.Create);
        }

        private void OnCancelCreateLobby()
        {
            SwitchPanel(LobbyPanel.Games);
        }

        private void PopulatePlayerList(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            if (lobby == null || lobby.Players == null)
            {
                Debug.LogWarning("Lobby is null or empty".Color("red"));
                ClearContainer(playerListContainer);
                return;
            }

            ClearContainer(playerListContainer);

            foreach (var player in lobby.Players)
            {
                string playerName = player.Data[KeyConstants.k_PlayerName].Value;
                
                string playerId = player.Data[KeyConstants.k_PlayerId].Value;
                
                var entry = Instantiate(playerEntryPrefab, playerListContainer).GetComponent<PlayerListItem>();
                
                entry.Initialize(playerName, playerId, new PlayerConfiguration
                {
                    Player = player,
                    IsHostPlayer = playerId == lobby.HostId,
                    IsLocalPlayer = playerId == AuthenticationService.Instance.PlayerId,
                });
                
                entry.gameObject.SetActive(true);
            }
        }

        private void PopulateLobbyList(List<Unity.Services.Lobbies.Models.Lobby> lobbies)
        {
            if (lobbies == null || lobbies.Count == 0)
            {
                Debug.LogWarning("No lobbies found".Color("red"));
                ClearContainer(lobbyListContainer);
                return;
            }
            
            ClearContainer(lobbyListContainer);

            foreach (var lobby in lobbies)
            {
                var entry = Instantiate(lobbyEntryPrefab, lobbyListContainer).GetComponent<GameListItem>();
                entry.UpdateLobby(lobby);
                entry.gameObject.SetActive(true);
            }
        }

        private void OnUpdateLobbyInfo(LobbyEventArgs lobbyEventArgs)
        {
            Debug.Log("On Update Lobby Info".Color("cyan"));

            gameName.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameName].Value;
            maxPlayers.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_MaxPlayers].Value;
            gameSpeed.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameSpeed].Value;
            gameMode.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameMode].Value;
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }

        private void SwitchPanel(LobbyPanel panel)
        {
            gamesListPanel.SetActive(false);
            createGamePanel.SetActive(false);
            lobbyPanel.SetActive(false);

            switch (panel)
            {
                case LobbyPanel.Games:
                    gamesListPanel.SetActive(true);
                    break;
                case LobbyPanel.Create:
                    createGamePanel.SetActive(true);
                    break;
                case LobbyPanel.Lobby:
                    lobbyPanel.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(panel), panel, null);
            }
        }
    }
}
