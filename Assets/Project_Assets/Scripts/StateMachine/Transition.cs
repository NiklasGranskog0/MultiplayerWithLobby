using Project_Assets.Scripts.Interfaces.StateMachine;

namespace Project_Assets.Scripts.StateMachine
{
    public class Transition : ITransition
    {
        public IState TargetState { get; }
        public ICondition Condition { get; }
        
        public Transition(IState targetState, ICondition condition)
        {
            TargetState = targetState;
            Condition = condition;
        }
    }
}
