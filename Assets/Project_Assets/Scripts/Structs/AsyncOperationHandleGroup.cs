using System.Collections.Generic;
using System.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Project_Assets.Scripts.Structs
{
    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> HandleGroups;
        public float Progress => HandleGroups.Count == 0 ? 0 : HandleGroups.Average(op => op.PercentComplete);
        public bool IsDone => HandleGroups.Count == 0 || HandleGroups.All(op => op.IsDone);

        public AsyncOperationHandleGroup(int initialSize)
        {
            HandleGroups = new List<AsyncOperationHandle<SceneInstance>>(initialSize);
        }
    }
}