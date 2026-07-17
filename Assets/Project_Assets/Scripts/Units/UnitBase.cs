using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class UnitBase : MonoBehaviour, ISelectionObject
    {
        public NavMeshAgent Agent;
        [HideInInspector] public string TeamTag;

        public StateMachine.StateMachine StateMachine;

        [HideInInspector] public GameManager GameManager;
        private GameMenuButtons m_gameMenuButtons;

        public virtual void Awake()
        {
            StateMachine = new StateMachine.StateMachine();
        }
        
        public virtual void Start()
        {
            // TODO: Maybe all units doesn't need a reference to the game manager
            ServiceLocator.ForSceneOf(this).Get(out GameManager);
            
            TeamTag = gameObject.tag;
            ServiceLocator.Global.Get(out m_gameMenuButtons);
        }

        public virtual void Update() => StateMachine.Update();
        

        public abstract ImageToLoad ImageToLoad { get; }
        public abstract string Name { get; }
        public virtual void SetGameMenuButtons() => m_gameMenuButtons.ResetButtonBinds();
    }
}