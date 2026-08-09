using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Scenes;
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

        public async void StartGame()
        {
            Debug.Log("GameManager StartGame".Color(Color.green));
            
            ServiceLocator.Global.Get(out SceneManager sm);
            await sm.SceneGroupManager.UnloadScene("Lobby");
            
            sm.SceneGroupManager.InvokeOnSceneGroupLoaded();
        }

        // private void Awake() => ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
    }
}