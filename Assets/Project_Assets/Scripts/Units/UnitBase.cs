using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Interfaces;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Assets.Scripts.Units
{
    //  TODO: maybe we can set team tag the same way we set it for the player?
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class UnitBase : NetworkBehaviour, ISelectionObject
    {
        public NavMeshAgent Agent;
        public NetworkVariable<FixedString32Bytes> TeamNetworkVariable = new();

        private void ApplyTeamTag(FixedString32Bytes team) => gameObject.tag = team.Value;
        private void OnTeamValueChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue) => ApplyTeamTag(newValue);

        protected StateMachine.StateMachine StateMachine;

        private GameMenuButtons m_gameMenuButtons;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            ApplyTeamTag(TeamNetworkVariable.Value);
            TeamNetworkVariable.OnValueChanged += OnTeamValueChanged;
        }

        public virtual void Awake()
        {
            StateMachine = new StateMachine.StateMachine();
        }
        
        public virtual void Start()
        {
            ServiceLocator.Global.Get(out m_gameMenuButtons);
        }

        public virtual void Update() => StateMachine.Update();
        

        public abstract ImageToLoad ImageToLoad { get; }
        public abstract string Name { get; }
        public virtual void SetGameMenuButtons() => m_gameMenuButtons.ResetButtonBinds();
    }
}