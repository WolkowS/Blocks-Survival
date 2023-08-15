using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumAttribute : DrawerAttribute
    {
        public Type _type;
		
        // =======================================================================
        public EnumAttribute(Type type)
        {
            _type = type;
        }
    }
}