using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private GameObject playerPrefab;
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost) return;
            
            foreach (var player in NetworkManager.Singleton.ConnectedClients)
            {
                SpawnPlayer(player.Value.ClientId);
            }
        }

        public void SpawnPlayer(ulong clientId)
        {
            var obj = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                playerPrefab.GetComponent<NetworkObject>(),
                clientId,
                false,
                true,
                false,
                playerSpawnPoint.position);
        }
    }
}
