using Project_Assets.Scripts.Interfaces.StateMachine;

namespace Project_Assets.Scripts.StateMachine.States
{
    public class BaseState : IState
    {
        public virtual void OnEnter() {}
        public virtual void OnExit() {}
        public virtual void OnUpdate() {}
    }
}