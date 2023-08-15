using System;

namespace CoreLib.Values.Condition
{
    [Serializable]
    public class LogicAnd : ConditionCheck
    {
        public ConditionCheck m_A;
        public ConditionCheck m_B;

        public override bool IsMet => m_A.IsMet && m_B.IsMet;
    }
}