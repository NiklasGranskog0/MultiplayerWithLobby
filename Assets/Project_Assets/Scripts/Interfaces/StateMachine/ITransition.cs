namespace Project_Assets.Scripts.Interfaces.StateMachine
{
    public interface ITransition
    {
        public IState TargetState { get; }
        public ICondition Condition { get; }
    }
}