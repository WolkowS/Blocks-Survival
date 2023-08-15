using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SOCollectionTypesAttribute : PropertyAttribute
    {
        public Type[] Types;

        public SOCollectionTypesAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}