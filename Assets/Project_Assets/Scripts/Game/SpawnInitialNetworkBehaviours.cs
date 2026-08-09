using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class SpawnInitialNetworkBehaviours : MonoBehaviour
    {
       [SerializeField] private GameObject[] m_networkBehaviours;

       private void Awake()
       {
           if (!NetworkManager.Singleton.IsHost) return;
           
           foreach (var behaviour in m_networkBehaviours)
           {
               // Extensions.CreateNetworkObjectAndSpawn(behaviour, Vector3.zero, NetworkManager.Singleton.LocalClientId);
               
               // var instance = Instantiate(behaviour);
               // var networkObject = instance.GetComponent<NetworkObject>();
               // networkObject.Spawn();
           }
       }
    }
}
