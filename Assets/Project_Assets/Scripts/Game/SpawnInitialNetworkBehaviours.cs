using Project_Assets.Scripts.UtilityExtensions.NetworkExtensions;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class SpawnInitialNetworkBehaviours : MonoBehaviour
    {
       [SerializeField] private GameObject[] m_networkBehaviours;

       private void Start()
       {
           if (!NetworkManager.Singleton.IsHost) return;

           foreach (var behaviour in m_networkBehaviours)
           {
               behaviour.CreateAsNetworkObjectAndSpawn(Vector3.zero, 0);
           }
       }
    }
}
