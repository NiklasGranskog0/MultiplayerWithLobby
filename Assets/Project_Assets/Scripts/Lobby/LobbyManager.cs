using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
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
        public event Action<LobbyEventArgs> OnJoinedLobbyUpdate;
        public event Action<LobbyListChangedEventArgs> OnLobbyListChanged;

        private readonly LobbyEventCallbacks m_EventCallbacks = new();

        public void UpdateActiveLobby(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            if (ActiveLobby.Players.Count == lobby.Players.Count) return;
            
            Debug.Log("LobbyManager.UpdateActiveLobby: Active Lobby less players".Color("red"));
            ActiveLobby = lobby;
                
            OnJoinedLobbyUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
        }

        private void LobbyUpdate(ILobbyChanges obj)
        {
            if (ActiveLobby == null) return;
            // Debug.Log("LobbyUpdate".Color("cyan"));
        }

        // Testing
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ListAllPlayersInLobby();
        }

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
            
            poller.OnShouldBeenKicked += PollerOnOnShouldBeenKicked;
        }

        private void PollerOnOnShouldBeenKicked(LobbyEventArgs obj)
        {
            OnPlayerLeftLobbyAsync?.Invoke(obj);
            poller.StopLobbyPolling();
        }

        public async Task<StatusReport> CreateLobbyAsync(CreateLobbySettings settings)
        {
            var statusReport = new StatusReport();
            var player = CreateLocalPlayer();

            var lobbyOptions = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = settings.IsPrivate,
                IsLocked = settings.IsLocked,
                Data = settings.Data,
            };

            // TODO: check max players and ?????

            if (string.IsNullOrWhiteSpace(settings.GameName.name))
            {
                statusReport.MakeReport(false, "Failed to create lobby, name was null");
                return statusReport;
            }

            try
            {
                ActiveLobby = await LobbyService.Instance.CreateLobbyAsync(settings.GameName.name,
                    settings.MaxPlayers.max, lobbyOptions);

                await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.Id, m_EventCallbacks);
                m_EventCallbacks.LobbyChanged += LobbyUpdate;

                heartbeat.StartHeartBeat(ActiveLobby.Id);
                poller.StartLobbyPolling(ActiveLobby);
                
                OnCreateLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnJoinedLobbyUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby }); // Populate Player List

                ListAllPlayersInLobby();

                statusReport.MakeReport(true, $"Lobby '{ActiveLobby.Name}' created with ID: {ActiveLobby.Id}");
            }
            catch (LobbyServiceException e)
            {
                statusReport.MakeReport(false, $"CreateLobbyAsync error (Failed to create lobby): {e.Message}");
            }

            return statusReport;
        }

        public async Task<StatusReport> LeaveLobbyAsync()
        {
            var statusReport = new StatusReport();

            if (ActiveLobby == null)
            {
                statusReport.MakeReport(false, "ActiveLobby not found (null)");
                return statusReport;
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
                OnJoinedLobbyUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

                // After UI have been changed to lobby list auto refresh active lobbies
                var lobbies = await GetAllActiveLobbiesAsync();
                lobbies.Log();

                statusReport.MakeReport(true, $"{playerId} left the lobby");
            }
            catch (LobbyServiceException e)
            {
                statusReport.MakeReport(false, $"{playerId} Failed to leave lobby: {e.Message}");
            }
            
            ActiveLobby = null;
            return statusReport;
        }

        public async Task<StatusReport> JoinLobbyByCodeAsync(string code)
        {
            var statusReport = new StatusReport();

            try
            {
                var player = CreateLocalPlayer();

                var joinOptions = new JoinLobbyByCodeOptions
                {
                    Player = player,
                };

                ActiveLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);
                poller.StartLobbyPolling(ActiveLobby);

                OnJoinedLobbyUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnPlayerJoinedLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

                statusReport.MakeReport(true, $"Joined lobby by code: {ActiveLobby.Name})");
            }
            catch (LobbyServiceException e)
            {
                statusReport.MakeReport(false, $"Lobby joined failed {e.Message}");
            }

            return statusReport;
        }

        public async Task<StatusReport> JoinLobbyByIdAsync(string lobbyId)
        {
            var statusReport = new StatusReport();

            try
            {
                var player = CreateLocalPlayer();

                var joinOptions = new JoinLobbyByIdOptions
                {
                    Player = player
                };

                ActiveLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinOptions);
                poller.StartLobbyPolling(ActiveLobby);

                OnPlayerJoinedLobbyAsync?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });
                OnJoinedLobbyUpdate?.Invoke(new LobbyEventArgs { Lobby = ActiveLobby });

                statusReport.MakeReport(true, $"Joined lobby by ID: {ActiveLobby.Id}");
            }
            catch (LobbyServiceException e)
            {
                statusReport.MakeReport(false, $"Failed to join lobby by ID: {e.Message}");
            }

            return statusReport;
        }

        public async Task<StatusReport> KickPlayerAsync(string playerId)
        {
            var statusReport = new StatusReport();
            
            if (ActiveLobby == null)
            {
                statusReport.MakeReport(false, "ActiveLobby not found (null)");
                return statusReport;
            }

            if (ActiveLobby.HostId != AuthenticationService.Instance.PlayerId)
            {
                statusReport.MakeReport(false, "Only the host can kick players");
                return statusReport;
            }

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.Id, playerId);
                
                statusReport.MakeReport(true, $"Player {playerId} has been kicked from the lobby");
            }
            catch (LobbyServiceException e)
            {
                statusReport.MakeReport(false, $"Failed to kick player: {e.Message}");
            }
            
            return statusReport;
        }

        private void ListAllPlayersInLobby()
        {
            if (ActiveLobby == null)
            {
                Debug.Log("No active lobby");
                return;
            }

            Debug.Log("Listing all players in lobby: ");

            foreach (var player in ActiveLobby.Players)
            {
                Debug.Log($"Name: {player.Data[KeyConstants.k_PlayerName].Value} Id: {player.Data[KeyConstants.k_PlayerId].Value}");
            }
        }

        /// <summary>
        /// Returns a Player object with metadata.
        /// Can't get Data if visibility is lower than public 
        /// </summary>
        private Player CreateLocalPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        KeyConstants.k_PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,
                            AuthenticationService.Instance.Profile)
                    },

                    {
                        KeyConstants.k_PlayerId, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,
                            AuthenticationService.Instance.PlayerId)
                    }
                }
            };
        }

        public async Task<LobbiesStatusReport> GetAllActiveLobbiesAsync()
        {
            var statusReport = new LobbiesStatusReport();

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

                statusReport.MakeReport(response.Results, true, $"Found {response.Results.Count} active lobbies.");
            }
            catch (LobbyServiceException e)
            {
                statusReport.MakeReport(null, false, $"Failed to query lobbies: {e.Message}");
            }

            return statusReport;
        }
    }
}