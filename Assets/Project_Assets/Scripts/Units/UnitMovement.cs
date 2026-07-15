using System.Collections;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : MonoBehaviour
    {
        public NavMeshAgent PlayerAgent;
        public string TeamTag;
        private GameManager m_gameManager;
        private Vector3 m_targetDestination;

        public void Initialize()
        {
            PlayerAgent.FixNavMeshNotFound();
            TeamTag = gameObject.tag;
            ServiceLocator.ForSceneOf(this).Get(out m_gameManager);

            m_targetDestination = TeamTag.Equals("Team1") ? m_gameManager.TeamTwoBase.transform.position : m_gameManager.TeamOneBase.transform.position;
            PlayerAgent.SetDestination(m_targetDestination);
            StartCoroutine(CheckDistanceLeft());    
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
