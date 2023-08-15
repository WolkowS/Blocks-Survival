using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AnimatorStateAttribute : DrawerAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimatorStateAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }
}