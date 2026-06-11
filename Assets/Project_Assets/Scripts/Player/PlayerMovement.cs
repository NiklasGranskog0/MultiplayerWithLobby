using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_playerAgent;
        [SerializeField] private LayerMask m_playerAgentLayer;

        private PlayerInputs m_playerInputs;
        private PlayerMouseTarget m_playerMouseTarget;

        public void Initialize(PlayerInputs playerInputs)
        { 
            m_playerInputs = playerInputs;
            ServiceLocator.Global.Get(out m_playerMouseTarget);

            // TODO: This is a hack to get the NavMeshAgent to work. (When agent is spawned it does not find navmesh)
            m_playerAgent.FixNavMeshNotFound();
            
            m_playerInputs.OnRightMouseClickEvent += RightMouseClickEvent;
        }
       
        private void RightMouseClickEvent()
        {
            // TODO: If distance to move is further than 'X' units, warp to new position
            if (Physics.Raycast(m_playerMouseTarget.MouseRay, out var hitInfo, float.MaxValue, m_playerAgentLayer))
            {
                m_playerAgent.SetDestination(hitInfo.point);
            }
        }
    }
}
