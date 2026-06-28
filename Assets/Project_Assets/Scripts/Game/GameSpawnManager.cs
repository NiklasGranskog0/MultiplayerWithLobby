using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Game
{
    public class GameSpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform[] m_spawnPoints;
        private Dictionary<ulong, Transform> m_availableSpawnPoints;
        private GameMenuButtons m_menuButtons;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
            m_availableSpawnPoints = new Dictionary<ulong, Transform>();
        }

        public Dictionary<ulong, Transform> SetPlayersSpawnPoint(List<ulong> players)
        {
            // TODO: 0, and equal number spawn on left side, and odd number spawn on right side
            for (int i = 0; i < players.Count; i++)
            {
                m_availableSpawnPoints.Add(players[i], m_spawnPoints[i]);
            }

            return m_availableSpawnPoints;
        }
    }
}
