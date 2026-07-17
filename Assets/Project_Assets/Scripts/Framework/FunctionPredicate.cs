using System;
using Project_Assets.Scripts.Interfaces.StateMachine;

namespace Project_Assets.Scripts.Framework
{
    public class FunctionPredicate : ICondition
    {
        private readonly Func<bool> m_predicate;

        public FunctionPredicate(Func<bool> predicate)
        {
            m_predicate = predicate;
        }

        public bool Evaluate() => m_predicate();
    }
}