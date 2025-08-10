using System;
using System.Collections;
using Project_Assets.Scripts.Events;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyPoller : MonoBehaviour
    {
        public float pollingInterval = 1f; // Fastest polling rate is 1f
        private Unity.Services.Lobbies.Models.Lobby CurrentLobby { get; set; }

        public event Action<LobbyEventArgs> OnShouldBeenKicked; 
        
        private Coroutine m_PollingCoroutine;

        private LobbyManager m_LobbyManager;
        
        private void Start()
        {
            ServiceLocator.Global.Get(out m_LobbyManager);
        }

        public void StartLobbyPolling(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            if (m_PollingCoroutine != null)
            {
                StopCoroutine(m_PollingCoroutine);
            }
            
            CurrentLobby = lobby;
            m_PollingCoroutine = StartCoroutine(PollLobbyCoroutine());
        }
        
        public void StopLobbyPolling()
        {
            if (m_PollingCoroutine != null)
            {
                StopCoroutine(m_PollingCoroutine);
                m_PollingCoroutine = null;
            }

            CurrentLobby = null;
        }
        
        private IEnumerator PollLobbyCoroutine()
        {
            while (CurrentLobby != null)
            {
                yield return new WaitForSeconds(pollingInterval);
                yield return FetchLobbyUpdate();
            }
        }
        
        private IEnumerator FetchLobbyUpdate()
        {
            var task = LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);

            while (!task.IsCompleted)
                yield return null;

            if (task.Exception != null)
            {
                Debug.LogWarning($"Lobby poll failed: {task.Exception.Message}".Color("red"));
                yield break;
            }

            if (CheckIfKicked())
            {
                OnShouldBeenKicked?.Invoke(new LobbyEventArgs { Lobby = CurrentLobby });
            }
            
            if (CurrentLobby == null)
            {
                Debug.Log("Lobby not found, stopping lobby polling".Color("red"));
                yield break;
            }
            
            var updatedLobby = task.Result;
            string oldHostId = CurrentLobby.HostId;
            string newHostId = updatedLobby.HostId;

            if (oldHostId != newHostId)
            {
                Debug.LogWarning("Host is missing. Attempting host reassignment...".Color("red"));
                yield return AssignNewHost(updatedLobby);
            }
            
            CurrentLobby = updatedLobby;
            Debug.Log($"Lobby polled. Host: {CurrentLobby.HostId}, Players: {CurrentLobby.Players.Count}".Color("cyan"));
        }

        private bool CheckIfKicked()
        {
            return !CurrentLobby.Players.Exists(p => p.Id == AuthenticationService.Instance.PlayerId);
        }
        
        // When new host has been assigned start a new heartbeat for the lobby
        private IEnumerator AssignNewHost(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            string myId = AuthenticationService.Instance.PlayerId;
            
            if (!lobby.Players.Exists(p => p.Id == myId)) yield break;

            string selectedHostId = lobby.Players[0].Id;
            
            if (myId != selectedHostId) yield break;

            var updateOptions = new UpdateLobbyOptions
            {
                HostId = selectedHostId
            };
            
            var task = LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateOptions);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Exception != null)
            {
                Debug.LogError($"Host reassignment failed: {task.Exception.Message}".Color("red"));
            }
            else
            { 
                Debug.Log($"Host reassignment succeeded {selectedHostId}".Color("green"));
                m_LobbyManager.ActiveLobby = task.Result;

                if (selectedHostId == myId)
                {
                    m_LobbyManager.heartbeat.StartHeartBeat(m_LobbyManager.ActiveLobby.Id);
                    Debug.Log("Started heartbeat (new host)".Color("cyan"));
                }
            }
        }
    }
}
