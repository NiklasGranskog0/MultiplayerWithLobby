using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.UnityServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator m_container;
        internal ServiceLocator Container => m_container.OrNull() ?? (m_container = GetComponent<ServiceLocator>());
        
        private bool m_isInitialized;

        private void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (m_isInitialized) return;
            m_isInitialized = true;
            Bootstrap();
        }
        
        protected abstract void Bootstrap();
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] private bool m_dontDestroyOnLoad = true;

        protected override void Bootstrap()
        {
           Container.ConfigureAsGlobal(m_dontDestroyOnLoad);
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