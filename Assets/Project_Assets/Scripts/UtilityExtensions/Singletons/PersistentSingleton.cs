using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.UtilityExtensions.Singletons
{
    public class PersistentNetworkSingleton<T> : NetworkBehaviour where T : Component
    {
        public bool UnParentOnAwake = true;
        public static bool HasInstance => s_instance != null;
        public static T Current => s_instance;

        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                
                s_instance = FindAnyObjectByType<T>();

                if (s_instance == null && Application.isPlaying)
                {
                    var newInstance = new GameObject()
                    {
                        name = typeof(T).Name + "AutoCreated",
                    }.AddComponent<T>();
                        
                    s_instance = newInstance;
                }

                return s_instance;
            }
        }

        // protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
        // {
        //     base.OnNetworkPreSpawn(ref networkManager);
        //     InitSingleton();
        // }

        private void Awake() => InitSingleton();

        protected virtual void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        private void InitSingleton()
        {
            if (!Application.isPlaying) return;
            if (UnParentOnAwake) transform.SetParent(null);

            if (s_instance == null)
            {
                s_instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (s_instance != this)
            {
                // If we are a networked object and the current instance is not, replace it
                var existingNO = (s_instance as Component).GetComponent<NetworkObject>();
                var myNO = GetComponent<NetworkObject>();

                if (myNO != null && existingNO == null)
                {
                    Destroy(s_instance.gameObject);
                    s_instance = this as T;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}