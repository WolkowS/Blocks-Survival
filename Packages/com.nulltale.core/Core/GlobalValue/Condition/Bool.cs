namespace CoreLib.Values.Condition
{
    public class Bool : ConditionCheck
    {
        public GvBool m_Value;

        public override bool IsMet => m_Value.Value;
    }
}