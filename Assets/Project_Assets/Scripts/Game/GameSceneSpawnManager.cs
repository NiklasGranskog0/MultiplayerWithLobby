using System;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.GlobalConstants.Strings;
using Project_Assets.Scripts.Lobby;
using Project_Assets.Scripts.Scenes;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    [Serializable]
    public struct PlayerObjects
    {
        public GameObject PlayerPrefab;
        public GameObject PlayerCameraPrefab;
    }

    public class GameSceneSpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_gameManagerPrefab;
        [SerializeField] private Transform[] m_spawnPoints;
        [SerializeField] private PlayerObjects m_playerObjects;

        private PlayersInLobby m_playersInLobby;
        private SceneManager m_sceneManager;

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost) return;
            // SpawnGameManager();

            ServiceLocator.Global.Get(out m_playersInLobby);
            ServiceLocator.Global.Get(out m_sceneManager);
            
            SpawnPlayers();
        }

        private void SpawnGameManager() =>
            Extensions.CreateNetworkObjectAndSpawn(m_gameManagerPrefab, Vector3.zero, 0); // 0 = host id

        private void SpawnPlayers()
        {
            // Creates a player and camera network object for each player in the lobby
            foreach (var player in m_playersInLobby.Players)
            {
                var data = player.Value.Data;
                var id = ulong.Parse(data[StringConstants.k_PlayerClientId].Value);
                var teamNb = ulong.Parse(data[StringConstants.k_PlayerTeam].Value);

                var playerObj = Extensions.CreateNetworkObjectAndSpawn(m_playerObjects.PlayerPrefab,
                    m_spawnPoints[teamNb].position, id);
                var playerCam = Extensions.CreateNetworkObjectAndSpawn(m_playerObjects.PlayerCameraPrefab,
                    m_spawnPoints[teamNb].position, id);

                // Set the player's and camera tag to their team number
                playerObj.gameObject.tag = Enum.GetName(typeof(Enums.Team), teamNb);
                playerCam.gameObject.tag = Enum.GetName(typeof(Enums.Team), teamNb);
            }
        }
    }
}