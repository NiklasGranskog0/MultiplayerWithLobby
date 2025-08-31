using System.Collections.Generic;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameSpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        private Dictionary<ulong, Transform> m_AvailableSpawnPoints;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
            m_AvailableSpawnPoints = new Dictionary<ulong, Transform>();
        }

        public Dictionary<ulong, Transform> SetPlayersSpawnPoint(List<ulong> players)
        {
            // TODO: Add an equal or more spawn points than players to prevent out of bounds
            // TODO: Bind the spawn point to the player
            for (int i = 0; i < players.Count; i++)
            {
                m_AvailableSpawnPoints.Add(players[i], spawnPoints[i]);
            }

            return m_AvailableSpawnPoints;
        }
    }
}
