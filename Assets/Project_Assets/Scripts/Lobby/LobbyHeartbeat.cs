using System.Collections;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Unity.Services.Lobbies;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyHeartbeat : MonoBehaviour
    {
        public float HeartBeatInterval = 15f; // Cannot be higher than 30
        private Coroutine m_heartBeatCoroutine;
        private string m_lobbyId;
    
        public void StartHeartBeat(string newLobbyId)
        {
            if (m_heartBeatCoroutine != null)
            {
                StopCoroutine(m_heartBeatCoroutine);
            }

            m_lobbyId = newLobbyId;
            m_heartBeatCoroutine = StartCoroutine(HeartbeatCoroutine());
        }

        public void StopHeartBeat()
        {
            if (m_heartBeatCoroutine != null)
            {
                StopCoroutine(m_heartBeatCoroutine);
                m_heartBeatCoroutine = null;
            }

            m_lobbyId = null;
        }

        private IEnumerator HeartbeatCoroutine()
        {
            while (!string.IsNullOrWhiteSpace(m_lobbyId))
            {
                yield return new WaitForSeconds(HeartBeatInterval);
                yield return SendHeartbeat();
            }
        }

        private IEnumerator SendHeartbeat()
        {
            var task = LobbyService.Instance.SendHeartbeatPingAsync(m_lobbyId);
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Exception != null)
            {
                Debug.LogError($"Heartbeat error: {task.Exception.Message}");
            }
            else
            {
                Debug.Log("Heartbeat sent".Color("cyan"));
            }
        }
    }
}
