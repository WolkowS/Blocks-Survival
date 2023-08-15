using System.Collections.Generic;
using System.Linq;

namespace CoreLib.ExposedValues
{
    public class ExposedConditionAll : ExposedCondition
    {
        public string m_Note;
        public List<ExposedCondition> m_Conditions;

        public override bool IsMet()
        {
            return m_Conditions.All(condition => condition.IsMet());
        }
    }
}