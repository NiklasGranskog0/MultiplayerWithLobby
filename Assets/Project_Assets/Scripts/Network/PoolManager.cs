using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Network
{
    public class PoolManager : NetworkBehaviour
    {
        private NetworkObjectPool m_networkObjectPool;
        private UnitTypeToPrefab m_unitTypeToPrefab;

        public override void OnNetworkSpawn()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_networkObjectPool);
        }
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start() => ServiceLocator.For(this).Get(out m_unitTypeToPrefab);

        [Rpc(SendTo.Server)]
        public void SpawnPooledObjectRpc(UnitType unitType, Vector3 spawnPoint, string team)
        {
            var prefab = m_unitTypeToPrefab.GetPrefabObject(unitType);
            var networkObject = m_networkObjectPool.GetNetworkObject(prefab, spawnPoint, Quaternion.identity);
            networkObject.gameObject.tag = team;
            networkObject.Spawn();
        }

        // TODO: Rpc can only serialize types with INetworkSerializable interface
        // [Rpc(SendTo.Server)]
        // public void ReturnPooledObjectRpc(NetworkObject networkObject)
        // {
        //    Reset object before returning it
        // }
    }
}
