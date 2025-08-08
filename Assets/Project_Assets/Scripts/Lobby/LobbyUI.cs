using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using Resources.MapImages;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        public Unity.Services.Lobbies.Models.Lobby CurrentSelectedLobby { get; set; }

        [Header("Panels")] [SerializeField] private GameObject gamesListPanel;
        [SerializeField] private GameObject createGamePanel;
        [SerializeField] private GameObject lobbyPanel;

        [Header("List Items")] [SerializeField]
        private GameObject lobbyEntryPrefab;

        [SerializeField] private Transform lobbyListContainer;

        [Space] [SerializeField] private GameObject playerEntryPrefab;
        [SerializeField] private Transform playerListContainer;

        [Space] [Header("Create Game Panel Elements")] [SerializeField]
        private TMP_InputField gameNameInputField;

        [SerializeField] private TMP_Dropdown gameModeDropdown;
        [SerializeField] private TMP_Dropdown maxPlayersDropdown;
        [SerializeField] private TMP_Dropdown mapDropdown;
        [SerializeField] private TMP_Dropdown visibilityDropdown;
        [SerializeField] private TMP_Dropdown gameSpeedDropdown;
        [SerializeField] private TMP_InputField crateGamePasswordInputField;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button cancelCreateButton;
        [SerializeField] private Button setMapImageButton;
        [SerializeField] private RawImage previewGameImage;

        [Header("Games Panel Elements")] public TMP_InputField gameCodeInputField;
        [SerializeField] private TMP_InputField gamePasswordInputField;
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button joinGameButton;
        [SerializeField] private Button refreshGamesButton;
        public LobbyInfo lobbyInfoGames;

        [Header("Lobby Panel Elements")] [SerializeField]
        private Button startGameButton;

        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private TMP_Text gameCodeText;
        [SerializeField] private LobbyInfo lobbyInfo;

        private int MaxPlayers => maxPlayersDropdown.value + 1;
        private int GameSpeedIndex => gameSpeedDropdown.value;
        private int GameModeIndex => gameModeDropdown.value;
        private int GameMapIndex => mapDropdown.value;
        private string GameName => gameNameInputField.text;

        private LobbyManager m_LobbyManager;
        private ErrorMessageText m_ErrorMessage;

        public ImagesDictionary gameImagesDictionary;

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }

        private void Start()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            ServiceLocator.Global.Get(out m_ErrorMessage);

            // Value == index
            maxPlayersDropdown.value = 3; // (Set default value to 4 players)

            createGameButton.onClick.AddListener(OnCreateLobbySettings);
            cancelCreateButton.onClick.AddListener(OnCancelCreateLobby);
            refreshGamesButton.onClick.AddListener(OnRefreshLobbies);
            createLobbyButton.onClick.AddListener(OnCreateLobby);
            leaveLobbyButton.onClick.AddListener(OnLeaveLobby);

            joinGameButton.onClick.AddListener(JoinSelectedGame);

            setMapImageButton.onClick.AddListener(OnSetMapImagePreview);

            visibilityDropdown.onValueChanged.AddListener(GameVisibilityChanged);
            crateGamePasswordInputField.interactable = false;

            m_LobbyManager.OnPlayerJoinedLobbyAsync += OnPlayerJoinedLobbyAsync;
            m_LobbyManager.OnPlayerLeftLobbyAsync += OnPlayerLeftLobbyAsync;
            m_LobbyManager.OnJoinedLobbyUpdate += OnJoinedLobbyUpdate;
            m_LobbyManager.OnLobbyListChanged += OnLobbyListChanged;
            m_LobbyManager.OnCreateLobbyAsync += OnCreateLobbyAsync;
            m_LobbyManager.OnSettingsUpdate += OnUpdateLobbyInfo;
            m_LobbyManager.OnSetGameCode += OnSetGameCode;

            OnRefreshLobbies();
        }

        private void OnSetMapImagePreview()
        {
            previewGameImage.color = Color.white;
            previewGameImage.texture = gameImagesDictionary["B"];
        }

        private void GameVisibilityChanged(int arg0)
        {
            crateGamePasswordInputField.interactable = visibilityDropdown.value == 1;
        }

        private void OnSetGameCode(string obj)
        {
            gameCodeText.text = obj;
        }

        private async void JoinSelectedGame()
        {
            var report = new StatusReport();
            bool joinedByCode = false;

            if (CurrentSelectedLobby == null)
            {
                joinedByCode = await TryJoinByCode();
            }

            if (!string.IsNullOrEmpty(CurrentSelectedLobby?.Id) && !joinedByCode)
            {
                report = await m_LobbyManager.JoinLobbyByIdAsync(CurrentSelectedLobby?.Id, gamePasswordInputField.text);
                PrintStatusLog(report, LobbyPanel.Games);
            }
        }

        private async Task<bool> TryJoinByCode()
        {
            if (!string.IsNullOrWhiteSpace(gameCodeInputField.text))
            {
                var report =
                    await m_LobbyManager.JoinLobbyByCodeAsync(gameCodeInputField.text, gamePasswordInputField.text);
                PrintStatusLog(report, LobbyPanel.Games);

                return report.Success;
            }

            return false;
        }

        private async void OnCreateLobby()
        {
            var settings = new CreateLobbySettings
            {
                IsLocked = false,
                IsPrivate = visibilityDropdown.value == 1,
                Password = crateGamePasswordInputField.text,

                GameImage = (previewGameImage, "B", DataObject.VisibilityOptions.Public),
                GameMode = ((GameMode)GameModeIndex, DataObject.VisibilityOptions.Public),
                GameMap = ((Map)GameMapIndex, DataObject.VisibilityOptions.Public),
                MaxPlayers = (MaxPlayers, DataObject.VisibilityOptions.Public),
                GameName = (GameName, DataObject.VisibilityOptions.Public),
                GameSpeed = ((GameSpeed)GameSpeedIndex, DataObject.VisibilityOptions.Public),
            };

            settings.SetData();

            lobbyInfo.gameName.text = settings.GameName.name;
            lobbyInfo.maxPlayers.text = settings.MaxPlayers.max.ToString();
            lobbyInfo.gameSpeed.text = settings.GameSpeed.speed.GameSpeedToString();
            lobbyInfo.gameMode.text = settings.GameMode.mode.GameModeToString();
            lobbyInfo.mapName.text = settings.GameMap.map.GameMapToString();

            lobbyInfo.gameImage.color = Color.white;
            lobbyInfo.gameImage.texture = settings.GameImage.image.texture;

            // TODO: Error message on password less than(<) 8 characters
            var report = await m_LobbyManager.CreateLobbyAsync(settings, crateGamePasswordInputField.interactable);
            PrintStatusLog(report, LobbyPanel.Create);
        }

        // Temp
        private async void OnRefreshLobbies()
        {
            var lobbiesStatusReport = await m_LobbyManager.GetAllActiveLobbiesAsync();
            PopulateLobbyList(lobbiesStatusReport.Lobbies);
            PrintStatusLog(lobbiesStatusReport.Status, LobbyPanel.Games);
        }

        private async void OnLeaveLobby()
        {
            var report = await m_LobbyManager.LeaveLobbyAsync();
            PrintStatusLog(report, LobbyPanel.Games);
            CurrentSelectedLobby = null;
        }

        private void OnLobbyListChanged(LobbyListChangedEventArgs e)
        {
            PopulateLobbyList(e.Lobbies);
            Debug.Log("Populate Lobby List (LobbyListChanged)".Color("cyan"));
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
            CurrentSelectedLobby = null;
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
            if (lobby?.Players == null)
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

            lobbyInfo.gameName.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameName].Value;
            lobbyInfo.maxPlayers.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_MaxPlayers].Value;
            lobbyInfo.gameSpeed.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameSpeed].Value;
            lobbyInfo.gameMode.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_GameMode].Value;
            lobbyInfo.mapName.text = lobbyEventArgs.Lobby.Data[KeyConstants.k_Map].Value;

            lobbyInfo.gameImage.color = Color.white;
            lobbyInfo.gameImage.texture =
                gameImagesDictionary[lobbyEventArgs.Lobby.Data[KeyConstants.k_GameImage].Value];
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }

        private void PrintStatusLog(StatusReport report, LobbyPanel panel)
        {
            if (!report.Success)
            {
                m_ErrorMessage.ShowError(report.Message, panel);
                return;
            }

            report.Log();
        }

        private void SwitchPanel(LobbyPanel panel)
        {
            gamesListPanel.SetActive(false);
            createGamePanel.SetActive(false);
            lobbyPanel.SetActive(false);

            gamePasswordInputField.text = string.Empty;
            gameCodeInputField.text = string.Empty;

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