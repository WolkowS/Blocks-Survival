using System.Collections.Generic;
using System.Linq;

namespace CoreLib.ExposedValues
{
    public class ExposedConditionAny : ExposedCondition
    {
        public List<ExposedCondition> m_Conditions;

        public override bool IsMet()
        {
            return m_Conditions.Any(condition => condition.IsMet());
        }
    }
}