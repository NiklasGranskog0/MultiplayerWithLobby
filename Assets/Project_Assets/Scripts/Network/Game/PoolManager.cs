using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Project_Assets.Scripts.Units;
using Project_Assets.Scripts.UtilityExtensions.Singletons;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Network.Game
{
    public class PoolManager : NetworkSingleton<PoolManager>
    {
        private UnitTypeToPrefab m_unitTypeToPrefab;

        private void Start()
        {
            ServiceLocator.For(this).Get(out m_unitTypeToPrefab);
        }

        [Rpc(SendTo.Server)]
        private void SpawnUnitRPC(UnitType unitType, Vector3 spawnPoint, string team)
        {
            Spawn(unitType, spawnPoint, team);
        }
        
        public void SpawnPooledObject(UnitType unitType, Vector3 spawnPoint, string team)
        {
            if (!IsHost)
            {
                SpawnUnitRPC(unitType, spawnPoint, team);
                return;
            }
            
            Spawn(unitType, spawnPoint, team);
        }

        private void Spawn(UnitType unitType, Vector3 spawnPoint, string team)
        {
            var prefab = m_unitTypeToPrefab.GetPrefabObject(unitType);
            var networkObject = NetworkObjectPool.Instance.GetNetworkObject(prefab, spawnPoint, Quaternion.identity);
            
            var unitBase = networkObject.GetComponent<UnitBase>();
            unitBase.TeamNetworkVariable.Value = (FixedString32Bytes)team;
            
            networkObject.Spawn();
        }
    }
}
