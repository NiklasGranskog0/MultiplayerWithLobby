using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Project_Assets.Scripts.Framework.HelperScripts
{
    public class PopulateNetworkPrefabList : MonoBehaviour
    {
        private static readonly NetworkPrefabHandler s_networkPrefabHandler = new ();
        
        [MenuItem("Tools/Populate Network Prefab List")]
        public static void Test()
        {
            foreach (var obj in Selection.gameObjects)
            {
                s_networkPrefabHandler.AddNetworkPrefab(obj);
            }
        }
    }
}
