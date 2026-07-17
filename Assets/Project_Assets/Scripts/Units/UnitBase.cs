using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class UnitBase : MonoBehaviour, ISelectionObject
    {
        public NavMeshAgent Agent { get; }
        public string TeamTag { get; }
        
        // [SerializeField] private UnitMovement m_unitMovement;
        // public string TeamTag;

        private StateMachine.StateMachine m_stateMachine;
        
        public virtual void Start()
        {
            // m_unitMovement.Initialize();
        }

        public abstract ImageToLoad ImageToLoad { get; }
        public abstract string Name { get; }
        public abstract void SetGameMenuButtons();
    }
}
