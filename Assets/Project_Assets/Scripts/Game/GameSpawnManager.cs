using System.Collections.Generic;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    // Player spawn manager
    public class GameSpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform[] m_spawnPoints;
        private Dictionary<ulong, Transform> m_availableSpawnPoints;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
            m_availableSpawnPoints = new Dictionary<ulong, Transform>();
        }

        public Dictionary<ulong, Transform> SetPlayersSpawnPoint(List<ulong> players)
        {
            // TODO: Add an equal or more spawn points than players to prevent out of bounds
            // TODO: Bind the spawn point to the player
            for (int i = 0; i < players.Count; i++)
            {
                m_availableSpawnPoints.Add(players[i], m_spawnPoints[i]);
            }

            return m_availableSpawnPoints;
        }
    }
}
