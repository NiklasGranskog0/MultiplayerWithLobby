using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Framework.ExtensionScripts
{
    public class NetworkSingleton<T> : NetworkBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance == null) Instance  = this as T;
            else
            {
                Destroy(this);
            }
        }

        // public static T Instance
        // {
        //     get
        //     {
        //         if (s_instance) return s_instance;
        //
        //         s_instance = FindAnyObjectByType<T>();
        //         
        //         s_instance = new GameObject
        //         {
        //             name = nameof(T),
        //             hideFlags = HideFlags.HideAndDontSave
        //         }.AddComponent<T>();
        //         
        //         Debug.Log("Singleton.cs Created new Singleton of type: " + typeof(T) +
        //                   ", Script calling to Singleton instance that does not exists.");
        //         
        //         return s_instance;
        //     }
        // }
    }
}
