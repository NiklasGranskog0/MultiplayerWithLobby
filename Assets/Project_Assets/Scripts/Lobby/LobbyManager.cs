using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_Assets.Scripts.Authentication;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Network.Relay;
using Project_Assets.Scripts.Structs;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public Unity.Services.Lobbies.Models.Lobby ActiveLobby;

        [SerializeField] public LobbyHeartbeat heartbeat;
        [SerializeField] public LobbyPoller poller;

        public event Action<LobbyEventArgs> OnCreateLobbyAsync;
        public event Action<LobbyEventArgs> OnPlayerLeftLobbyAsync;
        public event Action<LobbyEventArgs> OnPlayerJoinedLobbyAsync;
        public event Action<LobbyEventArgs> OnLobbyPlayerUpdate;
        public event Action<LobbyEventArgs> OnSettingsUpdate;
        public event Action<LobbyListChangedEventArgs> OnLobbyListChanged;
        public event Action<string> OnJoinedTextChannel;
        public event Action<string> OnLeftTextChannel;
        public event Action<string> OnSetGameCode;

        private static StatusReport s_statusReport;
        private static LobbiesStatusReport s_lobbiesStatusReport;
        private CreateLobbySettings m_CreateLobbySettings;

        private readonly LobbyEventCallbacks m_EventCallbacks = new();
        private PlayerAuthentication m_PlayerAuthentication;
        private RelayManager m_RelayManager;

        private void LobbyUpdate(ILobbyChanges obj)
        {
            if (ActiveLobby == null) return;

            try
            {
                obj.ApplyToLobby(ActiveLobby);
                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                Debug.Log("LobbyUpdate".Color("orange"));
            }
            catch (Exception e)
            {
                Debug.Log($"LobbyUpdate apply failed: {e.Message}".Color("red"));
            }
        }

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
            LobbyUI.onStartGame += StartGame;

            poller.OnShouldBeenKicked += PollerOnOnShouldBeenKicked;
        }

        private void Start()
        {
            ServiceLocator.Global.Get(out m_PlayerAuthentication);
            ServiceLocator.Global.Get(out m_RelayManager);
        }

        private void PollerOnOnShouldBeenKicked(LobbyEventArgs obj)
        {
            OnPlayerLeftLobbyAsync?.Invoke(obj);
            OnLeftTextChannel?.Invoke(obj.Lobby.Id);
            poller.StopLobbyPolling();
        }

        public async Task<StatusReport> CreateLobbyAsync(CreateLobbySettings settings, bool password)
        {
            var player = m_PlayerAuthentication.Player;

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

            try
            {
                ActiveLobby = await LobbyService.Instance.CreateLobbyAsync(settings.GameName.name,
                    settings.MaxPlayers.max, lobbyOptions);

                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_EventCallbacks);
                m_EventCallbacks.LobbyChanged -= LobbyUpdate;
                m_EventCallbacks.LobbyChanged += LobbyUpdate;

                heartbeat.StartHeartBeat(ActiveLobby.Id);
                poller.StartLobbyPolling(ActiveLobby);

                Debug.Log($"ActiveLobby Join Code: {ActiveLobby.LobbyCode}".Color("orange"));

                OnCreateLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnLobbyPlayerUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnJoinedTextChannel?.Invoke(ActiveLobby.Id);
                OnSetGameCode?.Invoke(ActiveLobby.LobbyCode);

                s_statusReport.MakeReport(true, $"Lobby '{ActiveLobby.Name}' created with ID: {ActiveLobby.Id}");
            }
            catch (LobbyServiceException e)
            {
                s_statusReport.MakeReport(false, $"CreateLobbyAsync error (Failed to create lobby): {e.Message}");
            }

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

            if (isHost && heartbeat != null)
            {
                heartbeat.StopHeartBeat();
                Debug.Log("Stopped heartbeat (was host)".Color("red"));
            }

            poller.StopLobbyPolling();

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.Id, playerId);

                OnPlayerLeftLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnLeftTextChannel?.Invoke(ActiveLobby.Id);

                // After UI have been changed to lobby list auto refresh active lobbies
                var lobbies = await GetAllActiveLobbiesAsync();
                lobbies.Log();

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
                var player = m_PlayerAuthentication.Player;

                var joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = player,
                };

                if (!string.IsNullOrWhiteSpace(password)) joinOptions.Password = password;

                ActiveLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);
                poller.StartLobbyPolling(ActiveLobby);

                // Subscribe to lobby events to receive changes (e.g., player data updates)
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_EventCallbacks);
                m_EventCallbacks.LobbyChanged -= LobbyUpdate;
                m_EventCallbacks.LobbyChanged += LobbyUpdate;

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

            return s_statusReport;
        }

        public async Task<StatusReport> JoinLobbyByIdAsync(string lobbyId, string password = null)
        {
            try
            {
                var player = m_PlayerAuthentication.Player;

                var joinOptions = new JoinLobbyByIdOptions
                {
                    Player = player,
                };

                if (!string.IsNullOrWhiteSpace(password)) joinOptions.Password = password;

                ActiveLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinOptions);
                poller.StartLobbyPolling(ActiveLobby);

                // Subscribe to lobby events to receive changes (e.g., player data updates)
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_EventCallbacks);
                m_EventCallbacks.LobbyChanged -= LobbyUpdate;
                m_EventCallbacks.LobbyChanged += LobbyUpdate;

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
                var updatePlayerOptions = new UpdatePlayerOptions
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

        private async void StartGame()
        {
            var maxPlayersString = ActiveLobby.Data[KeyConstants.k_MaxPlayers].Value;
            var maxPlayers = int.Parse(maxPlayersString);

            if (AuthenticationService.Instance.PlayerId != ActiveLobby.HostId) return;

            try
            {
                var relayStatus = await m_RelayManager.CreateRelay(maxPlayers - 1);

                ActiveLobby = await LobbyService.Instance.UpdateLobbyAsync(ActiveLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {KeyConstants.k_RelayCode, new DataObject(DataObject.VisibilityOptions.Member, relayStatus.JoinCode)},
                    }
                });
                
                relayStatus.Log();

                heartbeat.StopHeartBeat();
                poller.StopLobbyPolling();
                ActiveLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Start game failed: {e.Message}");
            }
        }

    // private void ListAllPlayersInLobby()
        // {
        //     if (ActiveLobby == null)
        //     {
        //         Debug.Log("No active lobby");
        //         return;
        //     }
        //
        //     Debug.Log("Listing all players in lobby: ");
        //
        //     foreach (var player in ActiveLobby.Players)
        //     {
        //         Debug.Log(
        //             $"Name: {player.Data[KeyConstants.k_PlayerName].Value} Id: {player.Data[KeyConstants.k_PlayerId].Value}");
        //     }
        // }

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

                OnLobbyListChanged?.Invoke(new LobbyListChangedEventArgs { Lobbies = response.Results });

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