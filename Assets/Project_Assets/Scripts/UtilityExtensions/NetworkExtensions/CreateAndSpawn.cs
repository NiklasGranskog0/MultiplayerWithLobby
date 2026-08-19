using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.UtilityExtensions.NetworkExtensions
{
    public static class CreateAndSpawn
    {
        public static NetworkObject CreateAsNetworkObjectAndSpawn(this GameObject prefab, Vector3 position, ulong clientId,
            bool isPlayerObject = false, bool destroyWithScene = false, bool forceOverride = false)
        {
            if (!prefab.TryGetComponent<NetworkObject>(out var networkObject))
            {
                Debug.LogError($"NetworkObject not found on prefab: {prefab.name}");
                return null;
            }

            return NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                networkObject,
                clientId,
                destroyWithScene,
                isPlayerObject,
                forceOverride,
                position);
        }
    }
}