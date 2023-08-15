using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ShowIfPlaymodeAttribute : ShowIfPlaymodeAttributeBase
    {
        public ShowIfPlaymodeAttribute() : base(true) { }
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ShowIfEditorAttribute : ShowIfPlaymodeAttributeBase
    {
        public ShowIfEditorAttribute() : base(false) { }
    }
    
    public class ShowIfPlaymodeAttributeBase : MetaAttribute
    {
        public bool IsPlaymode;
        public ShowIfPlaymodeAttributeBase(bool isPlaymode)
        {
            IsPlaymode = isPlaymode;
        }
    }
}