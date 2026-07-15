using System;

namespace Project_Assets.Scripts.Framework
{
    public class FunctionPredicate
    {
        private readonly Func<bool> m_predicate;
        
        public FunctionPredicate(Func<bool> predicate)
        {
            m_predicate = predicate;
        }

        public bool Evaluate() => m_predicate();
    }
}
