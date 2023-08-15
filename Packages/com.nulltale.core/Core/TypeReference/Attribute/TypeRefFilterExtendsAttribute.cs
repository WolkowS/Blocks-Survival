using System;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeRefFilterExtendsAttribute : TypeRefFilterAttribute
    {
        public bool					IncludeBaseType { get; set; }	= false;
        private Type				m_BaseType;

        // =======================================================================
        public TypeRefFilterExtendsAttribute(Type baseType)
        {
            m_BaseType = baseType.IsClass ? baseType : null;
        }

        public override bool Verify(Type type)
        {
            // check parent condition
            if (type.IsSubclassOf(m_BaseType) == false)
            {
                if (IncludeBaseType)
                    if (type == m_BaseType)
                        return true;

                return false;
            }

            return true;
        }
    }
}