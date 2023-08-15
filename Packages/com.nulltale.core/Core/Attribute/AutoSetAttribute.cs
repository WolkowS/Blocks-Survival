using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AutoSetAttribute : PropertyAttribute
    {
        public bool HideInInspector;

        public bool InSelf = true;
        public bool InParent;
        public bool InChildren;
        
        public string Path;
        
        public Type OfType;

        [Flags]
        public enum Source
        {
            Self     = 1,
            Parent   = 1 << 1,
            Chikdren = 1 << 2,
        }
        
        // =======================================================================
        public AutoSetAttribute()
        {
        }
        
        public AutoSetAttribute(Source source)
        {
            InSelf = source.HasFlag(Source.Self);
            InParent = source.HasFlag(Source.Parent);
            InChildren = source.HasFlag(Source.Chikdren);
        }
    }
}