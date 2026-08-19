using UnityEngine.AI;

namespace Project_Assets.Scripts.UtilityExtensions.NavMeshAgents
{
    public static class NavMesh
    {
        // TODO: This is a hack to get the NavMeshAgent to work. (When agent is spawned it does not find navmesh)
        public static void FixNavMeshNotFound(this NavMeshAgent agent)
        {
            agent.enabled = false;
            agent.enabled = true;
        }
    }
}