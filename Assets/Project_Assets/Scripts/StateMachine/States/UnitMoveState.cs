using System.Collections;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.StateMachine.States
{
    public class UnitMoveState : BaseState
    {
        private readonly NavMeshAgent m_agent;
        private readonly GameManager m_gameManager;
        private readonly GameObject m_unit;
        
        private Vector3 m_destination;
        private readonly string m_teamTag;

        public UnitMoveState(NavMeshAgent agent, string teamTag, GameManager gameManager, 
            GameObject unit)
        {
            m_unit = unit;
            m_agent = agent;
            m_teamTag = teamTag;
            m_gameManager = gameManager;
        }

        public override void OnEnter()
        {
            m_agent.FixNavMeshNotFound();
            
            SetDestination();
            m_unit.Get<MonoBehaviour>().StartCoroutine(CheckDistanceLeft());
        }

        public override void OnExit()
        {
            Debug.Log("Exiting Move State".Color(Color.lightSalmon));    
        }
        
        // public override void OnUpdate() { }
        
        private void SetDestination()
        {
            m_destination = m_teamTag.Equals("Team1")
                ? m_gameManager.TeamTwoBase.transform.position
                : m_gameManager.TeamOneBase.transform.position;
            
            m_agent.SetDestination(m_destination);
        }
        
        // Check if the player is close enough to the destination to stop the agent
        private IEnumerator CheckDistanceLeft()
        {
            var currentDistance = (m_unit.transform.position - m_agent.destination).magnitude;
            const float k_MaximumDistanceAway = 2f;
        
            while (currentDistance > k_MaximumDistanceAway)
            {
                currentDistance = (m_unit.transform.position - m_agent.destination).magnitude;
                yield return null;
            }
            
            if (currentDistance <= k_MaximumDistanceAway)
                m_agent.isStopped = true;
            
            yield return null;
        }
    }
}