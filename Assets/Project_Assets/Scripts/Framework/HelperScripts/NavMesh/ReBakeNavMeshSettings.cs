using Unity.AI.Navigation;
using UnityEngine;

namespace Project_Assets.Scripts.Framework.HelperScripts.NavMesh
{
    public class ReBakeNavMeshSettings : MonoBehaviour
    {
        // Layer mask for navmesh
        public LayerMask NavMeshLayer;

        // This will rebake all navmeshes in the scene.
        // It will also set the layer of the navmeshes to the given layer mask in the inspector,
        // except for the player plane navmesh.
        public void ReBake()
        {
            var allNavMeshes = FindObjectsByType<NavMeshSurface>();

            foreach (var navmesh in allNavMeshes)
            {
                if (navmesh.layerMask != LayerMask.NameToLayer("PlayerPlane"))
                {
                    navmesh.layerMask = NavMeshLayer;
                }

                navmesh.BuildNavMesh();
            }
        }
        
        public void AddNavMeshToLayer()
        {
            var allObjects = FindObjectsByType<GameObject>();
            
            foreach (var obj in allObjects)
            {
                if (obj.layer == LayerMask.NameToLayer("PlayerPlane")) continue;

                if (obj.layer == LayerMask.NameToLayer("Ground"))
                {
                    if (!obj.TryGetComponent(out NavMeshSurface _))
                    {
                        var objNavmesh = obj.AddComponent<NavMeshSurface>();
                        objNavmesh.layerMask = NavMeshLayer;
                        objNavmesh.BuildNavMesh();
                    }
                }
            }
        }

        public void RemoveNavMeshes()
        {
            var allObjects = FindObjectsByType<GameObject>();
            
            foreach (var obj in allObjects)
            {
                if (obj.layer == LayerMask.NameToLayer("PlayerPlane")) continue;

                if (obj.layer == LayerMask.NameToLayer("Ground"))
                {
                    DestroyImmediate(obj.GetComponent<NavMeshSurface>());
                }
            }
        }
    }
}