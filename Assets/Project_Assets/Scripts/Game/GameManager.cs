using System;
using System.Threading.Tasks;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Scenes;
using Project_Assets.Scripts.UtilityExtensions.Singletons;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : NetworkSingleton<GameManager>
    {
        public GameObject TeamOneBase;
        public GameObject TeamTwoBase;

        public override void OnNetworkSpawn()
        {
            Debug.Log("GameManager OnNetworkSpawn".Color(Color.green));
            
            // TODO: Set team bases prefab/game object
        }

        public async Task StartGame()
        {
            Debug.Log("GameManager StartGame".Color(Color.green));

            try
            {
                ServiceLocator.Global.Get(out SceneManager sm);
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