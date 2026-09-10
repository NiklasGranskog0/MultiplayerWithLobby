using System;
using Project_Assets.Scripts.Buildings;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.UtilityExtensions.Singletons;
using Project_Assets.Scripts.UtilityExtensions.Strings;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : NetworkSingleton<GameManager>
    {
        public GameObject TeamOneBase;
        public GameObject TeamTwoBase;
        
        public override void OnNetworkSpawn()
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));

            foreach (var baseTierOne in FindObjectsByType<BaseTierOne>())
            {
                if (baseTierOne.gameObject.tag.Equals("Team1"))
                    TeamOneBase = baseTierOne.gameObject;
                else
                    TeamTwoBase = baseTierOne.gameObject;
            }
            
            // TODO: Set team bases prefab/game object
        }

        public async void StartGame()
        {
            Debug.Log("GameManager StartGame".Color(Color.green));

            try
            {
                ServiceLocator.Global.Get(out Scenes.SceneManager sm);
                await sm.SceneGroupManager.UnloadScene("Lobby");

                sm.SceneGroupManager.InvokeOnSceneGroupLoaded();
            }
            catch (Exception e)
            {
                Debug.Log($"GameManager StartGame Failed: {e.Message}".Color(Color.red));
                throw;
            }
        }
    }
}