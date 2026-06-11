using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Game
{
    // Player spawn manager
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


        public Image TestImage;
        private void Start()
        {
            ServiceLocator.Global.Get(out m_menuButtons);
            
            m_menuButtons.BindButton(GameMenuButton.MiddleCenterLeft, TestFunction, TestImage, $"Test\nNewLineTest\nShortcut key: {nameof(KeyCode.C)}".Color("blue"), KeyCode.C);
        }

        private void TestFunction()
        {
            Debug.Log("Test Button Clicked".Color("orange"));
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
