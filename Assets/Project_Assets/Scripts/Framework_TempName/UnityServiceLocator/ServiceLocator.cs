using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_Assets.Scripts.Framework_TempName.UnityServiceLocator
{
    public enum ServiceLevel
    {
        Global,
        Scene,
        Local,
    }

    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator s_global;
        private static Dictionary<Scene, ServiceLocator> s_sceneContainers;
        private static List<GameObject> s_tempSceneGameObjects;

        private readonly ServiceManager m_Services = new();

        private const string k_GlobalServiceLocatorName = "ServiceLocator [Global]";
        private const string k_SceneServiceLocatorName = "ServiceLocator [Scene]";

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (s_global == this)
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            }
            else if (s_global != null)
            {
                Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator already configured as global",
                    this);
            }
            else
            {
                s_global = this;
                if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
        }

        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;
            if (s_sceneContainers.ContainsKey(scene))
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene",
                    this);
                return;
            }

            s_sceneContainers.Add(scene, this);
        }

        public static ServiceLocator Global
        {
            get
            {
                if (s_global != null) return s_global;

                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return s_global;
                }

                var container = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return s_global;
            }
        }

        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb);
        }

        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            var scene = mb.gameObject.scene;

            if (s_sceneContainers.TryGetValue(scene, out var container) && container != mb)
            {
                return container;
            }

            s_tempSceneGameObjects.Clear();
            scene.GetRootGameObjects(s_tempSceneGameObjects);

            foreach (var go in s_tempSceneGameObjects.Where(go =>
                         go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                if (go.TryGetComponent(out ServiceLocatorSceneBootstrapper sceneBootstrapper) &&
                    sceneBootstrapper.Container != mb)
                {
                    sceneBootstrapper.BootstrapOnDemand();
                    return sceneBootstrapper.Container;
                }
            }

            return Global;
        }

        public ServiceLocator Register<T>(T service, ServiceLevel level, string sceneName = null)
        {
            m_Services.Register(service);
            LogRegisterService(service, level, sceneName);
            return this;
        }

        public ServiceLocator Register(Type type, object service, ServiceLevel level, string sceneName = null)
        {
            m_Services.Register(type, service);
            LogRegisterService(service, level, sceneName);
            return this;
        }

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException("ServiceLocator: ".Color("red") +
                                        $"Service of type {typeof(T).Name.Color("red")} not registered!".Color(
                                            "lightblue"));
        }

        private bool TryGetService<T>(out T service) where T : class
        {
            return m_Services.TryGet(out service);
        }

        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == s_global)
            {
                container = null;
                return false;
            }

            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ??
                        ForSceneOf(this) ?? Global;
            return container != null;
        }

        private void OnDestroy()
        {
            if (this == s_global) s_global = null;
            else if (s_sceneContainers.ContainsValue(this))
            {
                s_sceneContainers.Remove(gameObject.scene);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_global = null;
            s_sceneContainers = new();
            s_tempSceneGameObjects = new();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobal()
        {
            var go = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        private static void AddScene()
        {
            var go = new GameObject(k_SceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif

        private void LogRegisterService<T>(T service, ServiceLevel level, string sceneName = "")
        {
            var endString = $"Registered service of the type {typeof(T).Name.Color("red")}".Color("lightblue");
            var globalString = "ServiceLocator ".Color("red") + $"[{level.ToString()}]".Color("orange") +
                               ": ".Color("red");
            var localOrScene = "ServiceLocator ".Color("red") + $"[{level + " | " + sceneName}]".Color("orange") +
                               ": ".Color("red");

            switch (level)
            {
                case ServiceLevel.Global:
                    Debug.Log(globalString + endString);
                    break;
                case ServiceLevel.Scene:
                    Debug.Log(localOrScene + endString);
                    break;
                case ServiceLevel.Local:
                    Debug.Log(localOrScene + endString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}