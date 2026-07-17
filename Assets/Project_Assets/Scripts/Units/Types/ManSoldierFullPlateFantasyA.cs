using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework;
using Project_Assets.Scripts.StateMachine.States;

namespace Project_Assets.Scripts.Units.Types
{
    public class ManSoldierFullPlateFantasyA : UnitBase
    {
        private UnitMoveState m_unitMoveState;
        private UnitGroundAttack m_unitGroundAttack;
        
        public override void Start()
        {
            base.Start();
            
            m_unitMoveState = new UnitMoveState(Agent, TeamTag, GameManager, gameObject);
            m_unitGroundAttack = new UnitGroundAttack();
            
            StateMachine.AddAnyTransition(m_unitMoveState, new FunctionPredicate(() => gameObject.activeInHierarchy));
            // StateMachine.AddTransition(m_unitMoveState, m_unitGroundAttack, new FunctionPredicate(() => );
            // StateMachine.AddTransition(m_AttackUnit, m_MoveToEnemyBaseState, new FunctionPredicate(() => NoUnitInRange);
            // StateMachine.AddAnyTransition(m_Dead, new FunctionPredicate(() => Health <= 0f);
            
            StateMachine.SetState(m_unitMoveState);
        }
        
        // private void Update() => StateMachine.Update();
        
        public override ImageToLoad ImageToLoad => ImageToLoad.ManSoldierFullPlateFantasyA;
        public override string Name => "Soldier";
        // public override void SetGameMenuButtons();
    }
}
