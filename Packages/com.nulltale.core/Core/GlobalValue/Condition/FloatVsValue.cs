using System;
using CoreLib.Scripting;

namespace CoreLib.Values.Condition
{
    [Serializable]
    public class FloatVsValue : ConditionCheck
    {
        public GvFloat m_Value;
        public float   m_Ref;

        public Comparison m_Check;

        public override bool IsMet => m_Check.Compare(m_Value.Value, m_Ref);
    }
}