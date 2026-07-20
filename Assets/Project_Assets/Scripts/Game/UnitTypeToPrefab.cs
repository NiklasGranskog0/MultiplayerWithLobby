using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Network;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    public class UnitTypeToPrefab : MonoBehaviour
    {
        private NetworkObjectPool m_networkObjectPool;
        
        [Serializable]
        public struct FromEnumToPrefab
        {
            public UnitType UnitType;
            public GameObject GameObject;
        }
        
        [SerializeField] private FromEnumToPrefab[] m_fromEnumToPrefab;
        private readonly Dictionary<UnitType, GameObject> m_unitTypeToPrefab = new();

        private void Awake() => ServiceLocator.For(this).Register(this, ServiceLevel.Local);

        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Get(out m_networkObjectPool);
            m_fromEnumToPrefab = new FromEnumToPrefab[m_networkObjectPool.PooledPrefabs.Count];

            for (int i = 0; i < m_networkObjectPool.PooledPrefabs.Count; i++)
            {
                var prefab = m_networkObjectPool.PooledPrefabs[i].Prefab;
                var unitType = prefab.name.UnitTypeFromString();
                
                m_fromEnumToPrefab[i].UnitType = unitType;
                m_fromEnumToPrefab[i].GameObject = prefab;
            }

            for (int i = 0; i < m_fromEnumToPrefab.Length; i++)
            {
                m_unitTypeToPrefab[m_fromEnumToPrefab[i].UnitType] = m_fromEnumToPrefab[i].GameObject;
            }
        }
        
        public GameObject GetPrefabObject(UnitType unitType)
        {
            return m_unitTypeToPrefab[unitType].gameObject;
        }
    }
}