namespace Project_Assets.Scripts.Interfaces.StateMachine
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void OnUpdate();
    }
}