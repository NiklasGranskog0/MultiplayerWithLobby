using System.Collections;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public NavMeshAgent PlayerAgent;
        [SerializeField] private LayerMask m_playerAgentLayer;
        private PlayerCamera m_playerCameraComponent;
        private PlayerInputs m_playerInputs;

        public void Initialize(PlayerInputs playerInputs, PlayerCamera playerCameraComponent)
        { 
            m_playerInputs = playerInputs;
            m_playerCameraComponent = playerCameraComponent; 

            // TODO: This is a hack to get the NavMeshAgent to work. (When agent is spawned it does not find navmesh)
            PlayerAgent.FixNavMeshNotFound();
            
            m_playerInputs.OnRightMouseClickEvent += RightMouseClickEvent;
        }
       
        private void RightMouseClickEvent()
        {
            PlayerAgent.isStopped = false;
            
            if (Physics.Raycast(m_playerCameraComponent.MouseRay, out var hitInfo, 500f, m_playerAgentLayer))
            {
                PlayerAgent.SetDestination(hitInfo.point);
                StartCoroutine(CheckDistanceLeft());
            }
        }

        // Check if the player is close enough to the destination to stop the agent
        private IEnumerator CheckDistanceLeft()
        {
            var currentDistance = (transform.position - PlayerAgent.destination).magnitude;
            const float k_MaximumDistanceAway = 2f;

            while (currentDistance > k_MaximumDistanceAway)
            {
                currentDistance = (transform.position - PlayerAgent.destination).magnitude;
                yield return null;
            }
            
            if (currentDistance <= k_MaximumDistanceAway)
                PlayerAgent.isStopped = true;
            
            yield return null;
        }
    }
}
