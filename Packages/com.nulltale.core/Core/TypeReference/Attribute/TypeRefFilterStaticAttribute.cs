using System;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeRefFilterStaticAttribute : TypeRefFilterAttribute
    {
        private bool m_StaticCondition;

        //////////////////////////////////////////////////////////////////////////
        public TypeRefFilterStaticAttribute(bool staticCondition)
        {
            m_StaticCondition = staticCondition;
        }

        public override bool Verify(Type type)
        {
            return (type.IsAbstract && type.IsSealed) == m_StaticCondition;
        }
    }
}