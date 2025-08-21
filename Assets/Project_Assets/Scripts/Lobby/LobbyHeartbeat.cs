using System.Collections;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Unity.Services.Lobbies;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class LobbyHeartbeat : MonoBehaviour
    {
        public float heartBeatInterval = 15f; // Cannot be higher than 30
        private Coroutine m_HeartBeatCoroutine;
        private string m_LobbyId;
    
        public void StartHeartBeat(string newLobbyId)
        {
            if (m_HeartBeatCoroutine != null)
            {
                StopCoroutine(m_HeartBeatCoroutine);
            }

            m_LobbyId = newLobbyId;
            m_HeartBeatCoroutine = StartCoroutine(HeartbeatCoroutine());
        }

        public void StopHeartBeat()
        {
            if (m_HeartBeatCoroutine != null)
            {
                StopCoroutine(m_HeartBeatCoroutine);
                m_HeartBeatCoroutine = null;
            }

            m_LobbyId = null;
        }

        private IEnumerator HeartbeatCoroutine()
        {
            while (!string.IsNullOrWhiteSpace(m_LobbyId))
            {
                yield return new WaitForSeconds(heartBeatInterval);
                yield return SendHeartbeat();
            }
        }

        private IEnumerator SendHeartbeat()
        {
            var task = LobbyService.Instance.SendHeartbeatPingAsync(m_LobbyId);
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
