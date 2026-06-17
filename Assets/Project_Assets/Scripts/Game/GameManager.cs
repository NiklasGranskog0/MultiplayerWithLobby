using System.Collections.Generic;
using System.Linq;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_playerPrefab; // TODO: TEMP

        private GameSpawnManager m_gameSpawnManager;
        private Dictionary<ulong, Transform> m_playersSpawnPoints;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost) return;

            ServiceLocator.ForSceneOf(this).Get(out m_gameSpawnManager);
            ServiceLocator.Global.Get(out PlayersInLobby playersInLobby);

            // TODO: PlayerTeam value 0 => Team 1, 1 => Team 2
            foreach (var data in playersInLobby.Players.Select(player => player.Value.Data))
            {
                Debug.Log($"Player: {data[KeyConstants.k_PlayerName].Value} - {data[KeyConstants.k_PlayerId].Value}\n" +
                          $"Team: {data[KeyConstants.k_PlayerTeam].Value}");
            }
            
            SetPlayersSpawnPoint();
            CreateAndSpawnPlayers();
        }

        // TODO: Can get NetworkClient from NetworkManager.Singleton.ConnectedClients 
        private void SetPlayersSpawnPoint()
        {
            var clientIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            m_playersSpawnPoints = m_gameSpawnManager.SetPlayersSpawnPoint(clientIds);
        }

        private void CreateAndSpawnPlayers()
        {
            foreach (var (clientId, spawnPoint) in m_playersSpawnPoints)
            {
                var playerObject = Extensions.CreateNetworkObject(m_playerPrefab, spawnPoint, clientId);
                // playerObject.gameObject.tag = nameof(Enums.Team.Team1);
            }
        }
    }
}