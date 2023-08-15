using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeRefFilterDisallowAttribute : TypeRefFilterAttribute
    {
        public List<Type>		m_DisallowList;

        //////////////////////////////////////////////////////////////////////////
        public TypeRefFilterDisallowAttribute(params Type[] typeList)
        {
            m_DisallowList = typeList.ToList();
        }

        public override bool Verify(Type type)
        {
            if (m_DisallowList.Contains(type))
                return false;

            return true;
        }
    }
}