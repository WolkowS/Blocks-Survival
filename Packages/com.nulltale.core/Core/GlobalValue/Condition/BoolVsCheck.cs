using System;

namespace CoreLib.Values.Condition
{
    public class BoolVsCheck : ConditionCheck
    {
        public GvBool m_Value;
        public ConditionBool   m_IsValue;

        public override bool IsMet => m_IsValue.Check(m_Value.Value);
    }
}