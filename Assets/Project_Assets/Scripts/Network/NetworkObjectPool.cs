using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace Project_Assets.Scripts.Network
{
    public class NetworkObjectPool : NetworkBehaviour
    {
        [SerializeField] private List<PoolConfigObject> m_pooledPrefabsList;
        public List<PoolConfigObject> PooledPrefabs => m_pooledPrefabsList;

        private readonly HashSet<GameObject> m_prefabs = new();

        private readonly Dictionary<GameObject, Queue<NetworkObject>> m_pooledObjects = new();

        private bool m_hasInitialized = false;

        public void Awake() =>
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);

        public override void OnNetworkSpawn() => InitializePool();
        public override void OnNetworkDespawn() => ClearPool();

        public void OnValidate()
        {
            for (var i = 0; i < m_pooledPrefabsList.Count; i++)
            {
                var prefab = m_pooledPrefabsList[i].Prefab;
                if (prefab != null)
                {
                    Assert.IsNotNull(
                        prefab.GetComponent<NetworkObject>(),
                        $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i.ToString()} has no {nameof(NetworkObject)} component."
                    );
                }

                var prewarmCount = m_pooledPrefabsList[i].PrewarmCount;
                if (prewarmCount < 0)
                {
                    Debug.LogWarning(
                        $"{nameof(NetworkObjectPool)}: Pooled prefab at index {i.ToString()} has a negative prewarm count! Making it not negative.");
                    var thisPooledPrefab = m_pooledPrefabsList[i];
                    thisPooledPrefab.PrewarmCount *= -1;
                    m_pooledPrefabsList[i] = thisPooledPrefab;
                }
            }
        }

        /// <summary>
        /// Gets an instance of the given prefab from the pool. The prefab must be registered to the pool.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public NetworkObject GetNetworkObject(GameObject prefab)
        {
            return GetNetworkObjectInternal(prefab, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Gets an instance of the given prefab from the pool. The prefab must be registered to the pool.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position">The position to spawn the object at.</param>
        /// <param name="rotation">The rotation to spawn the object with.</param>
        /// <returns></returns>
        public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return GetNetworkObjectInternal(prefab, position, rotation);
        }

        /// <summary>
        /// Return an object to the pool (reset objects before returning).
        /// Prefab is the key to the pool dictionary.
        /// </summary>
        public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
        {
            var go = networkObject.gameObject;
            go.SetActive(false);
            m_pooledObjects[prefab].Enqueue(networkObject);
        }

        /// <summary>
        /// Adds a prefab to the list of spawnable prefabs.
        /// </summary>
        /// <param name="prefab">The prefab to add.</param>
        /// <param name="prewarmCount"></param>
        public void AddPrefab(GameObject prefab, int prewarmCount = 0)
        {
            var networkObject = prefab.GetComponent<NetworkObject>();

            Assert.IsNotNull(networkObject, $"{nameof(prefab)} must have {nameof(networkObject)} component.");
            Assert.IsFalse(m_prefabs.Contains(prefab), $"Prefab {prefab.name} is already registered in the pool.");

            RegisterPrefabInternal(prefab, prewarmCount);
        }

        /// <summary>
        /// Builds up the cache for a prefab.
        /// </summary>
        private void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
        {
            m_prefabs.Add(prefab);

            var prefabQueue = new Queue<NetworkObject>();
            m_pooledObjects[prefab] = prefabQueue;
            for (int i = 0; i < prewarmCount; i++)
            {
                var go = CreateInstance(prefab);
                ReturnNetworkObject(go.GetComponent<NetworkObject>(), prefab);
            }

            // Register Netcode Spawn handlers
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject CreateInstance(GameObject prefab)
        {
            return Instantiate(prefab, transform);
        }

        /// <summary>
        /// This matches the signature of <see cref="NetworkSpawnManager.SpawnHandlerDelegate"/>
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private NetworkObject GetNetworkObjectInternal(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var queue = m_pooledObjects[prefab];

            var networkObject =
                queue.Count > 0 ? queue.Dequeue() : CreateInstance(prefab).GetComponent<NetworkObject>();

            // Here we must reverse the logic in ReturnNetworkObject.
            var go = networkObject.gameObject;
            go.SetActive(true);

            go.transform.position = position;
            go.transform.rotation = rotation;

            return networkObject;
        }

        /// <summary>
        /// Registers all objects in <see cref="m_pooledPrefabsList"/> to the cache.
        /// </summary>
        public void InitializePool()
        {
            Debug.Log("Initializing pool".Color(Color.lightSalmon));
            
            if (m_hasInitialized) return;
            foreach (var configObject in m_pooledPrefabsList)
            {
                RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
            }

            m_hasInitialized = true;
        }

        /// <summary>
        /// Unregisters all objects in <see cref="m_pooledPrefabsList"/> from the cache.
        /// </summary>
        public void ClearPool()
        {
            foreach (var prefab in m_prefabs)
            {
                // Unregister Netcode Spawn handlers
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
            }

            m_pooledObjects.Clear();
        }
    }

    [Serializable]
    public struct PoolConfigObject
    {
        public GameObject Prefab;
        public int PrewarmCount;
    }

    internal class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly GameObject m_prefab;
        private readonly NetworkObjectPool m_pool;

        public PooledPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
        {
            m_prefab = prefab;
            m_pool = pool;
        }

        NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position,
            Quaternion rotation)
        {
            var netObject = m_pool.GetNetworkObject(m_prefab, position, rotation);
            return netObject;
        }

        void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject)
        {
            m_pool.ReturnNetworkObject(networkObject, m_prefab);
        }
    }
}