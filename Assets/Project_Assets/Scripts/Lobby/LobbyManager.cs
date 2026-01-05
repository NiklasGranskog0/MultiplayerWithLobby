using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_Assets.Scripts.Authentication;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Network.Relay;
using Project_Assets.Scripts.Scenes;
using Project_Assets.Scripts.Structs;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public Unity.Services.Lobbies.Models.Lobby ActiveLobby;

        [SerializeField] public LobbyHeartbeat Heartbeat;
        [SerializeField] public LobbyPoller Poller;
        [SerializeField] public GameStartTimer GameStartTimer;
        private string m_lastSystemMessageSeen;

        public event Action<LobbyEventArgs> OnCreateLobbyAsync;
        public event Action<LobbyEventArgs> OnPlayerLeftLobbyAsync;
        public event Action<LobbyEventArgs> OnPlayerJoinedLobbyAsync;
        public event Action<LobbyEventArgs> OnLobbyPlayerUpdate;
        public event Action<LobbyEventArgs> OnSettingsUpdate;
        public event Action<LobbyListChangedEventArgs> LobbyListChanged;
        public event Action<string> OnJoinedTextChannel;
        public event Action<string> OnLeftTextChannel;
        public event Action<string> OnSetGameCode;
        public event Action<string> OnSendSystemMessage;

        private static StatusReport s_statusReport;
        private static LobbiesStatusReport s_lobbiesStatusReport;
        private CreateLobbySettings m_createLobbySettings;

        private readonly LobbyEventCallbacks m_eventCallbacks = new();
        private PlayerAuthentication m_playerAuthentication;
        private SceneManager m_sceneManager;
        private LobbyUI m_lobbyUI;
        private RelayManager m_relayManager;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
            LobbyUI.OnStartGame += StartCountdownTimer;

            Poller.OnShouldBeenKicked += PollerOnOnShouldBeenKicked;
            GameStartTimer.OnTimerLeft += SendTimerLeftMessage;
            GameStartTimer.OnTimerFinished += StartGame;
        }

        private void Start()
        {
            ServiceLocator.Global.Get(out m_playerAuthentication);
            ServiceLocator.Global.Get(out m_sceneManager);
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyUI);
            ServiceLocator.ForSceneOf(this).Get(out m_relayManager);
        }

        private async void LobbyUpdate(ILobbyChanges obj)
        {
            if (ActiveLobby == null) return;

            try
            {
                obj.ApplyToLobby(ActiveLobby);
                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

                // If a new system message is present, notify listeners at once
                if (ActiveLobby.Data != null &&
                    ActiveLobby.Data.TryGetValue(KeyConstants.k_SystemMessage, out var sysMsgObj))
                {
                    var msg = sysMsgObj?.Value;
                    if (!string.IsNullOrEmpty(msg) && msg != m_lastSystemMessageSeen)
                    {
                        // This message will always be called on game start, which means we want to load the game scene
                        m_lastSystemMessageSeen = msg;

                        // Only need to do this if we're not the host, because host will load game scene in StartGame()
                        if (AuthenticationService.Instance.PlayerId != ActiveLobby.HostId)
                        {
                            await m_sceneManager.LoadSceneGroupByEnum(SceneGroupToLoad.Game);
                        }
                    }
                }

                Debug.Log("LobbyUpdate".Color("orange"));
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"LobbyUpdate apply failed: {e.Message}".Color("red"));
            }
        }

        private async void StartCountdownTimer()
        {
            try
            {
                ActiveLobby = await LobbyService.Instance.UpdateLobbyAsync(ActiveLobby.Id, new UpdateLobbyOptions
                {
                    IsLocked = true
                });

                m_lobbyUI.StartGameButton.interactable = false;
                s_statusReport.MakeReport(true, "Lobby locked, Starting countdown timer...");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Failed to lock Lobby: {e.Message}");
                return;
            }

            s_statusReport.Log();
            GameStartTimer.StartTimer();
        }

        private void SendTimerLeftMessage(float obj)
        {
            OnSendSystemMessage?.Invoke($"Game starting in {obj}... ".Color("red"));
        }

        private void PollerOnOnShouldBeenKicked(LobbyEventArgs obj)
        {
            OnPlayerLeftLobbyAsync?.Invoke(obj);
            OnLeftTextChannel?.Invoke(obj.Lobby.Id);
            Poller.StopLobbyPolling();
        }

        public async Task<StatusReport> CreateLobbyAsync(CreateLobbySettings settings, bool password)
        {
            var player = m_playerAuthentication.Player;

            var lobbyOptions = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = settings.IsPrivate,
                IsLocked = settings.IsLocked,
                Data = settings.Data,
            };

            if (password)
            {
                lobbyOptions.Password = settings.Password;
            }

            if (string.IsNullOrWhiteSpace(settings.GameName.name))
            {
                s_statusReport.MakeReport(false, "Failed to create lobby, name was null");
                return s_statusReport;
            }

            OnCreateLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

            try
            {
                ActiveLobby = await LobbyService.Instance.CreateLobbyAsync(settings.GameName.name,
                    settings.MaxPlayers.max, lobbyOptions);

                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

                var relay = await m_relayManager.CreateRelay(settings.MaxPlayers.max - 1);
                relay.Log();

                var relayCodeUpdate = await UpdateRelayJoinCode(relay.JoinCode);
                relayCodeUpdate.Log();

                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_eventCallbacks);
                m_eventCallbacks.LobbyChanged -= LobbyUpdate;
                m_eventCallbacks.LobbyChanged += LobbyUpdate;

                s_statusReport.MakeReport(true, $"Lobby '{ActiveLobby.Name}' created with ID: {ActiveLobby.Id}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"CreateLobbyAsync error (Failed to create lobby): {e.Message}");
            }

            Heartbeat.StartHeartBeat(ActiveLobby.Id);
            Poller.StartLobbyPolling(ActiveLobby);

            OnJoinedTextChannel?.Invoke(ActiveLobby.Id);
            OnSetGameCode?.Invoke(ActiveLobby.LobbyCode);

            return s_statusReport;
        }

        public async Task<StatusReport> LeaveLobbyAsync()
        {
            if (ActiveLobby == null)
            {
                s_statusReport.MakeReport(false, "ActiveLobby not found (null)");
                return s_statusReport;
            }

            string playerId = AuthenticationService.Instance.PlayerId;
            bool isHost = playerId == ActiveLobby.HostId;

            if (isHost && Heartbeat != null)
            {
                Heartbeat.StopHeartBeat();
                Debug.Log("Stopped heartbeat (was host)".Color("red"));
            }

            Poller.StopLobbyPolling();

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.Id, playerId);

                OnPlayerLeftLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnLeftTextChannel?.Invoke(ActiveLobby.Id);

                if (AuthenticationService.Instance.PlayerId == playerId)
                {
                    NetworkManager.Singleton.Shutdown();
                }

                s_statusReport.MakeReport(true, $"{playerId} left the lobby");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"{playerId} Failed to leave lobby: {e.Message}");
            }

            ActiveLobby = null;
            return s_statusReport;
        }

        public async Task<StatusReport> JoinLobbyByCodeAsync(string code, string password = null)
        {
            try
            {
                var player = m_playerAuthentication.Player;

                var joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = player,
                };

                if (!string.IsNullOrWhiteSpace(password)) joinOptions.Password = password;

                ActiveLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);
                Poller.StartLobbyPolling(ActiveLobby);

                // Subscribe to lobby events to receive changes (e.g., player data updates)
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_eventCallbacks);
                m_eventCallbacks.LobbyChanged -= LobbyUpdate;
                m_eventCallbacks.LobbyChanged += LobbyUpdate;

                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnPlayerJoinedLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnSettingsUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnJoinedTextChannel?.Invoke(ActiveLobby.Id);
                OnSetGameCode?.Invoke(ActiveLobby.LobbyCode);

                s_statusReport.MakeReport(true, $"Joined lobby by code: {ActiveLobby.Name}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Lobby joined failed {e.Message}");
            }

            // TODO: When trying to join a lobby by typing in the name and there is no lobby already selected,
            // TODO: the active lobby instance is null and user won't join the relay or lobby
            JoinRelay(ActiveLobby.Data[KeyConstants.k_RelayCode].Value);
            return s_statusReport;
        }

        public async Task<StatusReport> JoinLobbyByIdAsync(string lobbyId, string password = null)
        {
            try
            {
                var player = m_playerAuthentication.Player;

                var joinOptions = new JoinLobbyByIdOptions
                {
                    Player = player,
                };

                if (!string.IsNullOrWhiteSpace(password)) joinOptions.Password = password;

                ActiveLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinOptions);
                Poller.StartLobbyPolling(ActiveLobby);

                // Subscribe to lobby events to receive changes (e.g., player data updates)
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_eventCallbacks);
                m_eventCallbacks.LobbyChanged -= LobbyUpdate;
                m_eventCallbacks.LobbyChanged += LobbyUpdate;

                OnPlayerJoinedLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnSettingsUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnJoinedTextChannel?.Invoke(ActiveLobby.Id);
                OnSetGameCode?.Invoke(ActiveLobby.LobbyCode);

                s_statusReport.MakeReport(true, $"Joined lobby by ID: {ActiveLobby.Id}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Failed to join lobby by ID: {e.Message}");
            }

            JoinRelay(ActiveLobby.Data[KeyConstants.k_RelayCode].Value);
            return s_statusReport;
        }

        public async Task<StatusReport> KickPlayerAsync(string playerId)
        {
            if (ActiveLobby == null)
            {
                s_statusReport.MakeReport(false, "ActiveLobby not found (null)");
                return s_statusReport;
            }

            if (ActiveLobby.HostId != AuthenticationService.Instance.PlayerId)
            {
                s_statusReport.MakeReport(false, "Only the host can kick players");
                return s_statusReport;
            }

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.Id, playerId);

                s_statusReport.MakeReport(true, $"Player {playerId} has been kicked from the lobby");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Failed to kick player: {e.Message}");
            }

            return s_statusReport;
        }

        public async Task<StatusReport> UpdateReadyButton(string playerId, bool isReady)
        {
            if (ActiveLobby == null)
            {
                s_statusReport.MakeReport(false, "ActiveLobby not found (null)");
                return s_statusReport;
            }

            try
            {
                var updatePlayerOptions = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            KeyConstants.k_PlayerReady,
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,
                                isReady.ToString().ToLower())
                        }
                    }
                };

                ActiveLobby =
                    await LobbyService.Instance.UpdatePlayerAsync(ActiveLobby.Id, playerId, updatePlayerOptions);

                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

                s_statusReport.MakeReport(true, $"Updated {playerId} ready state to: {isReady}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Failed to update ready state: {e.Message}");
            }

            return s_statusReport;
        }

        public async Task<StatusReport> UpdatePlayerTeamAsync(string playerId, int index)
        {
            if (ActiveLobby == null)
            {
                s_statusReport.MakeReport(false, "ActiveLobby not found (null)");
                return s_statusReport;
            }

            try
            {
                var updatePlayerOptions = new UpdatePlayerOptions()
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            KeyConstants.k_PlayerTeam,
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, index.ToString())
                        }
                    }
                };

                ActiveLobby =
                    await LobbyService.Instance.UpdatePlayerAsync(ActiveLobby.Id, playerId, updatePlayerOptions);

                s_statusReport.MakeReport(true, $"Updated {playerId} team index to: {index}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Failed to update player team: {e.Message}");
            }

            return s_statusReport;
        }

        private async Task<StatusReport> UpdateRelayJoinCode(string code)
        {
            try
            {
                ActiveLobby = await LobbyService.Instance.UpdateLobbyAsync(ActiveLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KeyConstants.k_RelayCode, new DataObject(DataObject.VisibilityOptions.Member, code)
                        }
                    }
                });

                s_statusReport.MakeReport(true, $"Updated relay join code to: {code}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"Failed to update relay join code: {e.Message}");
            }

            return s_statusReport;
        }

        private async void StartGame()
        {
            try
            {
                // Send a system message to all players via lobby data update
                var message = "GameStarting";
                ActiveLobby = await LobbyService.Instance.UpdateLobbyAsync(ActiveLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KeyConstants.k_SystemMessage, new DataObject(DataObject.VisibilityOptions.Member, message)
                        }
                    }
                });

                m_sceneManager.SwitchLoadingScreen(LoadingScreenEnum.Game);
                m_sceneManager.SetLoadingScreenTitle(ActiveLobby.Name);
                await m_sceneManager.LoadSceneGroupByEnum(SceneGroupToLoad.Game);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Start game failed: {e.Message}");
            }
        }

        private async void JoinRelay(string code)
        {
            var relay = await m_relayManager.JoinRelay(code);
            relay.Log();
        }

        public async Task<LobbiesStatusReport> GetAllActiveLobbiesAsync()
        {
            try
            {
                var queryOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        // Optional filters, e.g., public lobbies only
                        new QueryFilter(
                            field: QueryFilter.FieldOptions.AvailableSlots,
                            op: QueryFilter.OpOptions.GT,
                            value: "0")
                    },

                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created) // Sort by newest first
                    }
                };

                var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

                LobbyListChanged?.Invoke(new LobbyListChangedEventArgs { Lobbies = response.Results });

                s_lobbiesStatusReport.MakeReport(response.Results, true,
                    $"Found {response.Results.Count} active lobbies.");
            }
            catch (LobbyServiceException e)
            {
                s_lobbiesStatusReport.MakeReport(null, false, $"Failed to query lobbies: {e.Message}");
            }

            return s_lobbiesStatusReport;
        }
    }
}