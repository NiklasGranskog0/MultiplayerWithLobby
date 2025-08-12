using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.SerializedDictionaries;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Network.Relay;
using Project_Assets.Scripts.Structs;
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
        public Unity.Services.Lobbies.Models.Lobby CurrentLobby { get; set; }

        [Header("Panels")] 
        [SerializeField] private GameObject gamesListPanel;
        [SerializeField] private GameObject createGamePanel;
        [SerializeField] private GameObject lobbyPanel;

        [Header("List Items")] 
        [SerializeField] private GameObject lobbyEntryPrefab;
        [SerializeField] private Transform lobbyListContainer;

        [Space(5)] 
        
        [SerializeField] private GameObject playerEntryPrefab;
        [SerializeField] private Transform playerListContainer;

        [Space] [Header("Create Game Panel Elements")] 
        [SerializeField] private TMP_InputField gameNameInputField;
        [SerializeField] private TMP_Dropdown gameModeDropdown;
        [SerializeField] private TMP_Dropdown maxPlayersDropdown;
        [SerializeField] private TMP_Dropdown mapDropdown;
        [SerializeField] private TMP_Dropdown visibilityDropdown;
        [SerializeField] private TMP_Dropdown gameSpeedDropdown;
        [SerializeField] private TMP_InputField createGamePasswordInputField;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button cancelCreateButton;
        [SerializeField] private Button setMapImageButton;
        [SerializeField] private RawImage previewGameImage;

        [Header("Games Panel Elements")] 
        public TMP_InputField gameCodeInputField;
        [SerializeField] private TMP_InputField gamePasswordInputField;
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button joinGameButton;
        [SerializeField] private Button refreshGamesButton;
        public LobbyInfo lobbyInfoGames;

        [Header("Lobby Panel Elements")] 
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private TMP_Text gameCodeText;
        [SerializeField] private LobbyInfo lobbyInfo;

        private int MaxPlayers => maxPlayersDropdown.value + 1;
        private int GameSpeedIndex => gameSpeedDropdown.value;
        private int GameModeIndex => gameModeDropdown.value;
        private int GameMapIndex => mapDropdown.value;
        private string GameName => gameNameInputField.text;
        private Dictionary<Player, string> m_PlayersReadyValues = new();

        private LobbyManager m_LobbyManager;
        private ErrorMessageText m_ErrorMessage;
        private RelayManager m_RelayManager;

        public ImagesDictionary gameImagesDictionary;
        private string m_GameImageName;
        [SerializeField] private Button tempQuitButton;
        

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }

        private void Start()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
            ServiceLocator.Global.Get(out m_ErrorMessage);
            ServiceLocator.Global.Get(out m_RelayManager);

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
            createGamePasswordInputField.interactable = false;

            m_LobbyManager.OnPlayerJoinedLobbyAsync += OnPlayerJoinedLobbyAsync;
            m_LobbyManager.OnPlayerLeftLobbyAsync += OnPlayerLeftLobbyAsync;
            m_LobbyManager.OnLobbyPlayerUpdate += OnLobbyPlayerUpdate;
            m_LobbyManager.OnLobbyListChanged += OnLobbyListChanged;
            m_LobbyManager.OnCreateLobbyAsync += OnCreateLobbyAsync;
            m_LobbyManager.OnSettingsUpdate += OnUpdateLobbyInfo;
            m_LobbyManager.OnSetGameCode += OnSetGameCode;

            tempQuitButton.onClick.AddListener(QuitLobby);
            startGameButton.onClick.AddListener(OnHostStartGame);
            startGameButton.interactable = false;
            
            OnRefreshLobbies();
        }

        private async void OnHostStartGame()
        {
            int maxPlayers = 3;
            
            SwitchPanel(LobbyPanel.Loading);
            var task = await m_RelayManager.CreateRelay(maxPlayers);
            task.Log();
        }

        private void QuitLobby()
        {
            Application.Quit();
        }

        private void OnSetMapImagePreview()
        {
            m_GameImageName = "B";
            previewGameImage.color = Color.white;
            previewGameImage.texture = gameImagesDictionary[m_GameImageName];
        }

        private void GameVisibilityChanged(int arg0)
        {
            createGamePasswordInputField.interactable = visibilityDropdown.value == 1;
        }

        private void OnSetGameCode(string obj)
        {
            gameCodeText.text = obj;
        }

        private async void JoinSelectedGame()
        {
            bool joinedByCode = false;

            if (CurrentSelectedLobby == null)
            {
                joinedByCode = await TryJoinByCode();
            }

            if (!string.IsNullOrEmpty(CurrentSelectedLobby?.Id) && !joinedByCode)
            {
                var task = await m_LobbyManager.JoinLobbyByIdAsync(CurrentSelectedLobby?.Id, gamePasswordInputField.text);
                PrintStatusLog(task, LobbyPanel.GamePanel);
            }
        }

        private async Task<bool> TryJoinByCode()
        {
            if (!string.IsNullOrWhiteSpace(gameCodeInputField.text))
            {
                var task =
                    await m_LobbyManager.JoinLobbyByCodeAsync(gameCodeInputField.text, gamePasswordInputField.text);
                PrintStatusLog(task, LobbyPanel.GamePanel);

                return task.Success;
            }

            return false;
        }

        private async void OnCreateLobby()
        {
            var settings = new CreateLobbySettings
            {
                IsLocked = false,
                IsPrivate = visibilityDropdown.value == 1,
                Password = createGamePasswordInputField.text,

                GameImage = (previewGameImage, m_GameImageName, DataObject.VisibilityOptions.Public),
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

            if (createGamePasswordInputField.text.Length < 8 && visibilityDropdown.value == 1)
            {
                m_ErrorMessage.ShowError("Password must be at least 8 characters long", LobbyPanel.CreatePanel);
                return;
            }
            
            var task = await m_LobbyManager.CreateLobbyAsync(settings, createGamePasswordInputField.interactable);
            PrintStatusLog(task, LobbyPanel.CreatePanel);
        }

        // Temp
        private async void OnRefreshLobbies()
        {
            var task = await m_LobbyManager.GetAllActiveLobbiesAsync();
            PopulateLobbyList(task.Lobbies);
            PrintStatusLog(task.Status, LobbyPanel.GamePanel);
        }

        private async void OnLeaveLobby()
        {
            var task = await m_LobbyManager.LeaveLobbyAsync();
            PrintStatusLog(task, LobbyPanel.GamePanel);
            CurrentSelectedLobby = null;
        }

        private void OnLobbyListChanged(LobbyListChangedEventArgs e)
        {
            PopulateLobbyList(e.Lobbies);
            Debug.Log("Populate Lobby List (LobbyListChanged)".Color("cyan"));
        }

        private void OnLobbyPlayerUpdate(LobbyEventArgs e)
        {
            UpdatePlayerList(e.Lobby);
            Debug.Log("Populate Player List".Color("cyan"));
        }

        private void OnPlayerJoinedLobbyAsync(LobbyEventArgs obj)
        {
            SwitchPanel(LobbyPanel.Lobby);
            CurrentLobby = obj.Lobby;
            Debug.Log("Player Joined Lobby".Color("cyan"));
        }

        private void OnCreateLobbyAsync(LobbyEventArgs obj)
        {
            SwitchPanel(LobbyPanel.Lobby);
            CurrentLobby = obj.Lobby;
        }

        private void OnPlayerLeftLobbyAsync(LobbyEventArgs obj)
        {
            SwitchPanel(LobbyPanel.GamePanel);
            Debug.Log($"Player Left; Lobby Id: {obj.Lobby.Id}");
            CurrentSelectedLobby = null;
            CurrentLobby = null;
        }

        private void OnCreateLobbySettings()
        {
            SwitchPanel(LobbyPanel.CreatePanel);
        }

        private void OnCancelCreateLobby()
        {
            SwitchPanel(LobbyPanel.GamePanel);
        }

        private void UpdatePlayerList(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            if (lobby?.Players == null)
            {
                Debug.LogWarning("Lobby players is null or empty".Color("red"));
                ClearContainer(playerListContainer);
                return;
            }

            ClearContainer(playerListContainer);
            
            RemoveKickedPlayerFromReadyValues(lobby);
            
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
                
                var localIsHost = AuthenticationService.Instance.PlayerId == lobby.HostId;

                if (!localIsHost)
                {
                    startGameButton.gameObject.SetActive(false);
                }
                
                var readyValue = player.Data[KeyConstants.k_PlayerReady].Value;

                if (localIsHost)
                {
                    if (!m_PlayersReadyValues.TryAdd(player, readyValue))
                    {
                        m_PlayersReadyValues[player] = readyValue;
                    }
                }
            }

            UpdateStartGameButton();
        }

        private void RemoveKickedPlayerFromReadyValues(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            foreach (var player in m_PlayersReadyValues.Keys.ToList().Where(player => !lobby.Players.Contains(player)))
            {
                m_PlayersReadyValues.Remove(player);
            }
        }

        private void UpdateStartGameButton()
        {
            if (m_PlayersReadyValues.Where(playerReady => playerReady.Value != "true").Any(player => player.Value == "false"))
            {
                startGameButton.interactable = false;
                return;
            }

            startGameButton.interactable = true;
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

            if (lobbyEventArgs.Lobby.Data[KeyConstants.k_GameImage].Value != null)
            {
                lobbyInfo.gameImage.texture =
                    gameImagesDictionary[lobbyEventArgs.Lobby.Data[KeyConstants.k_GameImage].Value];
            }
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
            {
                if (child != null)
                    Destroy(child.gameObject);
            }
        }

        private void PrintStatusLog(StatusReport task, LobbyPanel panel)
        {
            if (!task.Success)
            {
                m_ErrorMessage.ShowError(task.Message, panel);
                return;
            }

            task.Log();
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
                case LobbyPanel.GamePanel:
                    gamesListPanel.SetActive(true);
                    break;
                case LobbyPanel.CreatePanel:
                    createGamePanel.SetActive(true);
                    break;
                case LobbyPanel.Lobby:
                    lobbyPanel.SetActive(true);
                    break;
                case LobbyPanel.Loading:
                    lobbyPanel.SetActive(false);
                    break;
                case LobbyPanel.Game:
                    lobbyPanel.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(panel), panel, null);
            }
        }
    }
}