using System;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeRefFilterTemplateAttribute : TypeRefFilterAttribute
    {
        private bool m_TemplateCondition;

        //////////////////////////////////////////////////////////////////////////
        public TypeRefFilterTemplateAttribute(bool templateCondition)
        {
            m_TemplateCondition = templateCondition;
        }

        public override bool Verify(Type type)
        {
            return type.IsGenericType == m_TemplateCondition;
        }
    }
}