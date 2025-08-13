using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.ExtensionScripts
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance) return s_instance;

                s_instance = FindFirstObjectByType<T>();
                
                s_instance = new GameObject
                {
                    name = nameof(T),
                    hideFlags = HideFlags.HideAndDontSave
                }.AddComponent<T>();
                
                Debug.Log("Singleton.cs Created new Singleton of type: " + typeof(T) +
                          ", Script calling to Singleton instance that does not exists.");
                
                return s_instance;
            }
        }
        
        private void OnDestroy() => DestroySingleton();
        
        private void DestroySingleton()
        {
            if (s_instance == this)
                s_instance = null;
        }
    }
}
