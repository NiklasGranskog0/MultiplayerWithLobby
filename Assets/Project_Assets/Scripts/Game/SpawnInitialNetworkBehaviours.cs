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
            
            foreach (var behaviour in m_networkBehaviours)
            {
                behaviour.CreateAsNetworkObjectAndSpawn(Vector3.zero, 0);
                Debug.Log($"Host spawned: {behaviour.name}".Color(Color.lightSalmon));
            }

            IsComplete = true;
        }
    }
}