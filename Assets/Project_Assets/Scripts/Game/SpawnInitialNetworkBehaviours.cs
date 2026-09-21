using Project_Assets.Scripts.Network.Game;
using Project_Assets.Scripts.UtilityExtensions.NetworkExtensions;
using Project_Assets.Scripts.UtilityExtensions.Strings;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class SpawnInitialNetworkBehaviours : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_networkBehaviours;
        public bool IsComplete { get; private set; } = false;
        
        public void LoadGameScripts()
        {
            if (!NetworkManager.Singleton.IsHost) return; 
            
            Debug.Log("Creating network behaviours".Color(Color.lightSalmon));
            foreach (var behaviour in m_networkBehaviours)
            {
                Debug.Log($"Creating: {behaviour.name}".Color(Color.lightSalmon));
                behaviour.CreateAsNetworkObjectAndSpawn(Vector3.zero, 0);
            }

            Debug.Log("Creating players".Color(Color.lightSalmon));
            var playerCreator = FindObjectsByType<PlayerCreator>()[0];
            playerCreator.CreatePlayers();
            
            IsComplete = true;
        }
    }
}