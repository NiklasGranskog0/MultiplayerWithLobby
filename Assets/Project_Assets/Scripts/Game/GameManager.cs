using System.Collections.Generic;
using System.Linq;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab; // TODO: TEMP

        private GameSpawnManager m_GameSpawnManager;
        private Dictionary<ulong, Transform> m_PlayersSpawnPoints;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost) return;

            ServiceLocator.ForSceneOf(this).Get(out m_GameSpawnManager);
            SetPlayersSpawnPoint();

            CreateAndSpawnPlayers();
        }

        private void SetPlayersSpawnPoint()
        {
            var ids = NetworkManager.Singleton.ConnectedClients.Select(player => player.Key).ToList();
            m_PlayersSpawnPoints = m_GameSpawnManager.SetPlayersSpawnPoint(ids);
        }

        private void CreateAndSpawnPlayers()
        {
            foreach (var id in m_PlayersSpawnPoints)
            {
                Extensions.CreateNetworkObject(playerPrefab, id.Value, id.Key);
            }
        }
    }
}