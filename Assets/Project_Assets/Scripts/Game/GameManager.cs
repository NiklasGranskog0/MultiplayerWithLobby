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
            SetPlayersSpawnPoint();

            CreateAndSpawnPlayers();
        }

        private void SetPlayersSpawnPoint()
        {
            var ids = NetworkManager.Singleton.ConnectedClients.Select(player => player.Key).ToList();
            m_playersSpawnPoints = m_gameSpawnManager.SetPlayersSpawnPoint(ids);
        }

        private void CreateAndSpawnPlayers()
        {
            foreach (var id in m_playersSpawnPoints)
            {
                Extensions.CreateNetworkObject(m_playerPrefab, id.Value, id.Key);
            }
        }
    }
}