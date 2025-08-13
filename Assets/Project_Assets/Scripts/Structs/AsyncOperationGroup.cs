using System.Collections.Generic;
using System.Linq;

namespace Project_Assets.Scripts.Structs
{
    public readonly struct AsyncOperationGroup
    {
        public readonly List<UnityEngine.AsyncOperation> AsyncOperations;
        public float Progress => AsyncOperations.Count == 0 ? 0 : AsyncOperations.Average(op => op.progress);
        public bool IsDone => AsyncOperations.All(op => op.isDone);

        public AsyncOperationGroup(int initialSize)
        {
            AsyncOperations = new List<UnityEngine.AsyncOperation>(initialSize);
        }
    }
}