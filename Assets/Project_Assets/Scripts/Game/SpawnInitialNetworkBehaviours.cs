using Project_Assets.Scripts.Framework.ExtensionScripts;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class SpawnInitialNetworkBehaviours : MonoBehaviour
    {
       [SerializeField] private GameObject[] m_networkBehaviours;

       private void Awake()
       {
           foreach (var behaviour in m_networkBehaviours)
           {
               Extensions.CreateNetworkObject(behaviour);
           }
       }
    }
}
