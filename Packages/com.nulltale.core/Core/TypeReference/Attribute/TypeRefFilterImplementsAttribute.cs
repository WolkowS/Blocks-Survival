using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeRefFilterImplementsAttribute : TypeRefFilterAttribute
    {
        private List<Type>		m_ImplementList;

        // =======================================================================
        public TypeRefFilterImplementsAttribute(params Type[] interfaceList)
        {
            m_ImplementList = interfaceList.Where(type => type.IsInterface).ToList();
        }

        public override bool Verify(Type type)
        {
            // check interface condition
            var interfaceList = type.GetInterfaces();
            if (m_ImplementList.All(intr => interfaceList.Contains(intr)) == false)
                return false;

            return true;
        }
    }
}