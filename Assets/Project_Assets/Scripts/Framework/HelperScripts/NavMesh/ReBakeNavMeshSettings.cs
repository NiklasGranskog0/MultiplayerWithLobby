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
    }
}