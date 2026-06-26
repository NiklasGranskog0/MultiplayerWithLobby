using System;
using System.Collections.Generic;
using System.Linq;
using Project_Assets.Scripts.Interfaces.StateMachine;

namespace Project_Assets.Scripts.StateMachine
{
    public class StateMachine
    {
        private StateNode m_currentStateNode;
        private readonly Dictionary<Type, StateNode> m_stateNodes = new();
        private readonly HashSet<ITransition> m_anyTransitions = new();

        public void Update()
        {
            var transitions = GetTransition();
            if (transitions is not null) ChangeState(transitions.TargetState);
            m_currentStateNode.State.OnUpdate();
        }

        public void SetState(IState state)
        {
            m_currentStateNode = m_stateNodes[state.GetType()];
            m_currentStateNode.State.OnEnter();
        }

        public void AddTransition(IState fromState, IState targetState, ICondition condition) =>
            GetOrAddStateNode(fromState).AddTransition(GetOrAddStateNode(targetState).State, condition);
        
        public void AddAnyTransition(IState targetState, ICondition condition) =>
            m_anyTransitions.Add(new Transition(GetOrAddStateNode(targetState).State, condition));

        private StateNode GetOrAddStateNode(IState state)
        {
            var stateNode = m_stateNodes.GetValueOrDefault(state.GetType());

            if (stateNode != null) return stateNode;
            
            stateNode = new StateNode(state);
            m_stateNodes.Add(state.GetType(), stateNode);

            return stateNode;
        }

        private ITransition GetTransition()
        {
            foreach (var transition in m_anyTransitions.Where(transition => transition.Condition.Evaluate()))
            {
                return transition;
            }

            return m_currentStateNode.Transitions.FirstOrDefault(transition => transition.Condition.Evaluate());
        }

        private void ChangeState(IState newState)
        {
            if (newState == m_currentStateNode.State) return;
            
            var previousState = m_currentStateNode.State;
            var nextState = m_stateNodes[newState.GetType()].State;
            
            previousState.OnExit();
            nextState.OnEnter();
            m_currentStateNode = m_stateNodes[newState.GetType()];
        }
        
        private class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState targetState, ICondition condition)
            {
                Transitions.Add(new Transition(targetState, condition));
            }
        }      
    }
}
