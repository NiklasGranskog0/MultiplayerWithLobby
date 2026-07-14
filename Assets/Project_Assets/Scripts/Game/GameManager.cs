using System;
using System.Collections.Generic;
using System.Linq;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Lobby;
using Project_Assets.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        // TODO: Get scriptable object with prefabs (network prefabs for now)
        // TODO: Enum To Prefab serialized dictionary 
        [SerializeField] private GameObject m_playerPrefab;
        [SerializeField] private GameObject m_playerCameraPrefab;
        [SerializeField] private GameObject m_unitPrefabTest;
        [SerializeField] private Transform m_castleSpawnPositionTest;

        private GameSpawnManager m_gameSpawnManager;
        private Dictionary<ulong, Transform> m_playersSpawnPoints;
        private PlayersInLobby m_playersInLobby;

        public void SpawnTestPrefab(ulong clientId)
        {
            Extensions.CreateNetworkObject(m_unitPrefabTest, m_castleSpawnPositionTest, clientId);
        }

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost) return;

            ServiceLocator.ForSceneOf(this).Get(out m_gameSpawnManager);
            ServiceLocator.Global.Get(out m_playersInLobby);
            
            SetPlayersSpawnPoint();
            CreateAndSpawnPlayers();
        }

        // TODO: Should not be in GameManager
        private void SetPlayersSpawnPoint()
        {
            var clientIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            
            // TODO: Only need 2 spawn points, left & right
            // TODO: Could do spawn areas, and spawn players in a random position in that area
            m_playersSpawnPoints = m_gameSpawnManager.SetPlayersSpawnPoint(clientIds); // Temp
        }

        // TODO: Spawn should not be in GameManager
        private void CreateAndSpawnPlayers()
        {
            // Creates a player and camera network object for each player in the lobby
            foreach (var player in m_playersInLobby.Players)
            {
                var data = player.Value.Data;
                var id = ulong.Parse(data[StringConstants.k_PlayerClientId].Value);
                var teamNb = ulong.Parse(data[StringConstants.k_PlayerTeam].Value);
                
                var playerObj = Extensions.CreateNetworkObject(m_playerPrefab, m_playersSpawnPoints[teamNb].transform, id);
                var playerCam = Extensions.CreateNetworkObject(m_playerCameraPrefab, m_playersSpawnPoints[teamNb].transform, id);
                
                // Set the player's and camera tag to their team number
                playerObj.gameObject.tag = Enum.GetName(typeof(Enums.Team), teamNb);
                playerCam.gameObject.tag = Enum.GetName(typeof(Enums.Team), teamNb);
            }
        }
    }
}