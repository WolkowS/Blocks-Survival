using System;
using System.Linq;
using SoCreator;

namespace CoreLib.Values.Condition
{
    [SOCollectionIgnore] [SoCreate(true, true)]
    public class ConditionGroup : ConditionCheck
    {
        public SoCollection<ConditionCheck> m_Checks;

        /// <summary>
        /// returns true if empty or all conditions was met
        /// </summary>
        public override bool IsMet => m_Checks.Values.Any(n => n.IsMet == false) == false;
    }
}