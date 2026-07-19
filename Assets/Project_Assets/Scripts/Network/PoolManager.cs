using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Unity.Netcode;

namespace Project_Assets.Scripts.Network
{
    public class PoolManager : NetworkBehaviour
    {
        private NetworkObjectPool m_networkObjectPool;

        public override void OnNetworkSpawn()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_networkObjectPool);
            
            if (NetworkManager.Singleton.IsHost)
            {
                InitializePoolRpc();
            }
        }
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }
        
        [Rpc(SendTo.Everyone)]
        private void InitializePoolRpc()
        {
            m_networkObjectPool.InitializePool();  
        } 
    }
}
