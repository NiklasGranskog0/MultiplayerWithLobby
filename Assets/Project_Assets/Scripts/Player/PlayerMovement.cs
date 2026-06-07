using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private NavMeshAgent m_playerAgent;
        [SerializeField] private LayerMask m_playerAgentLayer;

        private PlayerInputs m_playerInputs;
        private Vector3 m_mousePosition;
        private Ray m_mouseRay => m_playerCamera.ScreenPointToRay(m_mousePosition);

        public void Initialize(PlayerInputs playerInputs)
        { 
            m_playerInputs = playerInputs;

            // TODO: This is a hack to get the NavMeshAgent to work. (When agent is spawned it does not find navmesh)
            m_playerAgent.FixNavMeshNotFound();
            
            m_playerInputs.OnMouseMovedEvent += MouseMoved;
            m_playerInputs.OnRightMouseClickEvent += RightMouseClickEvent;
        }
        
        private void MouseMoved(Vector2 mousePosition)
        {
            m_mousePosition = mousePosition;
        }

        private void RightMouseClickEvent()
        {
            // TODO: If distance to move is further than 'X' units, warp to new position
            
            if (Physics.Raycast(m_mouseRay, out var hitInfo, float.MaxValue, m_playerAgentLayer))
            {
                m_playerAgent.SetDestination(hitInfo.point);
            }
        }
    }
}
