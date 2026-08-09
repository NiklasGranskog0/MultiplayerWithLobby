using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class PlayersLoadState : NetworkBehaviour
    {
        private LobbyManager m_lobbyManager;
        private PlayersInLobby m_playersInLobby;
        private int m_loadedPlayerCount;
        private bool m_gameStarted;

        public override void OnNetworkSpawn()
        {
            Debug.Log("PlayersLoadState: OnNetworkSpawn".Color(Color.orange));
        }

        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_lobbyManager);
            ServiceLocator.Global.Get(out m_playersInLobby);
            m_lobbyManager.OnSendPlayerLoadState += OnSendPlayerLoadState;
        }

        private void OnSendPlayerLoadState(string playerId, SceneEventType loadState)
        {
            Debug.Log($"PlayerClientId: {playerId},  LoadState: {loadState}".Color(Color.green));

            if (!NetworkManager.IsHost)
            {
                ClientLoadCompleteRPC();
            }
            else
            {
                OwnerLoadCompleteRPC();
            }
        }

        private void CheckAllPlayersLoaded()
        {
            if (!m_loadedPlayerCount.Equals(m_playersInLobby.Players.Count)) return;
            
            if (!m_gameStarted)
            {
                StartGameRPC();
            }
        }

        // Clients send message to Host
        [Rpc(SendTo.Owner)]
        private void ClientLoadCompleteRPC()
        {
            m_loadedPlayerCount++;
            CheckAllPlayersLoaded();
        }

        // Host sends message to himself
        [Rpc(SendTo.Owner)]
        private void OwnerLoadCompleteRPC()
        {
            m_loadedPlayerCount++;
            CheckAllPlayersLoaded();
        }

        [Rpc(SendTo.Everyone)]
        private void StartGameRPC()
        {
            m_gameStarted = true;
            Debug.Log("StartGameRPC".Color(Color.green));
            
            GameManager.Instance.StartGame();
        }
    }
}