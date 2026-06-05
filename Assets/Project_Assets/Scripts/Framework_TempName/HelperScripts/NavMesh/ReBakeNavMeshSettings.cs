using Unity.AI.Navigation;
using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.HelperScripts.NavMesh
{
    public class ReBakeNavMeshSettings : MonoBehaviour
    {
        // Layer mask for navmesh
        public LayerMask NavMeshLayer;
        
        // Re bake all navmeshes with the same layer mask
        public void ReBake()
        {
            var allNavMeshes = FindObjectsByType<NavMeshSurface>();
            
            foreach (var navmesh in allNavMeshes)
            {
                navmesh.layerMask = NavMeshLayer;
                navmesh.BuildNavMesh();
            }
        }
    }
}
