using System.Collections.Generic;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Network;
using Project_Assets.Scripts.Units.Types;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_unitPrefabTest;
        [SerializeField] private GameObject m_poolNetworkManager;
        
        [SerializeField] private Transform m_castleSpawnPositionTeam1;
        [SerializeField] private Transform m_castleSpawnPositionTeam2;
        
        public GameObject TeamOneBase;
        public GameObject TeamTwoBase;

        private Dictionary<ulong, Transform> m_playersSpawnPoints;
        private NetworkObjectPool m_networkObjectPool;

        public void SpawnTestPrefab(ulong clientId, string teamTag)
        {
            var spawnPoint = teamTag == "Team1" ? m_castleSpawnPositionTeam1 : m_castleSpawnPositionTeam2;

            if (NetworkManager.Singleton.IsHost)
            {
                var networkObject = m_networkObjectPool.GetNetworkObject(m_unitPrefabTest, spawnPoint.position, Quaternion.identity);
                networkObject.gameObject.tag = teamTag;
                networkObject.Spawn(); 
            }
            else
            {
                SpawnRpc(teamTag, spawnPoint.position);
            }
        }

        public void ReturnTestPrefab()
        {
            foreach (var obj in GameObject.FindGameObjectsWithTag("Team1"))
            {
                if (!obj.TryGetComponent<ManSoldierFullPlateFantasyA>(out var soldier)) continue;
                
                m_networkObjectPool.ReturnNetworkObject(soldier.GetComponent<NetworkObject>(), m_unitPrefabTest);
            }
        }

        //[Rpc(SendTo.Server)]
        private void SpawnRpc(string teamTag, Vector3 spawnPoint)
        {
            var networkObject = m_networkObjectPool.GetNetworkObject(m_unitPrefabTest, spawnPoint, Quaternion.identity);
            networkObject.gameObject.tag = teamTag;
            networkObject.Spawn();
        }

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
            // Extensions.CreateNetworkObject(m_poolNetworkManager, NetworkManager.Singleton.LocalClientId);
        }

        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_networkObjectPool);
            if (!NetworkManager.Singleton.IsHost) return;
        }
    }
}