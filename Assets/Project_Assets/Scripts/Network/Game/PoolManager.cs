using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Project_Assets.Scripts.UtilityExtensions.Singletons;
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

        public void SpawnPooledObject(UnitType unitType, Vector3 spawnPoint, string team)
        {
            var prefab = m_unitTypeToPrefab.GetPrefabObject(unitType);
            var networkObject = NetworkObjectPool.Instance.GetNetworkObject(prefab, spawnPoint, Quaternion.identity);
            networkObject.gameObject.tag = team;
            networkObject.Spawn();
        }
    }
}
