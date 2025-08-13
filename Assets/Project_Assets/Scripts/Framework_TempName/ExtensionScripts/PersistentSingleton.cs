using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.ExtensionScripts
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public bool unParentOnAwake = true;
        public static bool HasInstance => s_instance != null;
        public static T Current => s_instance;

        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                
                s_instance = FindFirstObjectByType<T>();

                if (s_instance == null)
                {
                    var newInstance = new GameObject()
                    {
                        name = typeof(T).Name + "AutoCreated",
                        hideFlags = HideFlags.HideAndDontSave,
                    }.AddComponent<T>();
                        
                    s_instance = newInstance;
                }

                return s_instance;
            }
        }
        
        protected virtual void Awake() => InitSingleton();

        protected virtual void InitSingleton()
        {
            if (!Application.isPlaying) return;
            if (unParentOnAwake) transform.SetParent(null);

            if (s_instance == null)
            {
                s_instance = this as T;
                DontDestroyOnLoad(gameObject);
                enabled = true;
            }
            else
            {
                if (this != s_instance) Destroy(gameObject);
            }
        }
    }
}