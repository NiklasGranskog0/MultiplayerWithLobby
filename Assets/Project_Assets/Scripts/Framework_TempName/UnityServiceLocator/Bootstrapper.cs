using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.UnityServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator m_Container;
        internal ServiceLocator Container => m_Container.OrNull() ?? (m_Container = GetComponent<ServiceLocator>());
        
        private bool m_IsInitialized;

        private void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (m_IsInitialized) return;
            m_IsInitialized = true;
            Bootstrap();
        }
        
        protected abstract void Bootstrap();
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] private bool dontDestroyOnLoad = true;

        protected override void Bootstrap()
        {
           Container.ConfigureAsGlobal(dontDestroyOnLoad);
        }
    }
    
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        // [SerializeField] private bool dontDestroyOnLoad = true;

        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}