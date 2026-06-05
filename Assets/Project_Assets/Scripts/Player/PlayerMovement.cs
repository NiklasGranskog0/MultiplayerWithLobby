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
        // [SerializeField] private float m_movementSpeed = 5f;

        private Ray m_mouseRay => m_playerCamera.ScreenPointToRay(m_mousePosition);
        private PlayerInputs m_playerInputs;

        private Vector3 m_mousePosition;

        public void Initialize(PlayerInputs playerInputs)
        { 
            m_playerInputs = playerInputs;

            // TODO: This is a hack to get the NavMeshAgent to work. (When agent is spawned it does not find navmesh)
            m_playerAgent.enabled = false;
            m_playerAgent.enabled = true;
            
            m_playerInputs.OnMouseMovedEvent += MouseMoved;
            m_playerInputs.OnRightMouseClickEvent += RightMouseClickEvent;
        }
        
        private void MouseMoved(Vector2 mousePosition)
        {
            m_mousePosition = mousePosition;
        }

        private void RightMouseClickEvent()
        {
            if (Physics.Raycast(m_mouseRay, out var hitInfo, float.MaxValue, m_playerAgentLayer))
            {
                m_playerAgent.SetDestination(hitInfo.point);
            }
        }
    }
}
