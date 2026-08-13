using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.UtilityExtensions.Singletons
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
    }
}
