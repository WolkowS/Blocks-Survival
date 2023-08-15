using System;

namespace CoreLib
{
    [Serializable]
    public enum ConditionValue
    {
        Positive,
        Negative,
        Zero,
        Any,
    }

    public static class ConditionValueExtentions
    {
        public static bool Check(this ConditionValue comparation, int val)
        {
            return comparation switch
            {
                ConditionValue.Positive => val > 0,
                ConditionValue.Negative => val < 0,
                ConditionValue.Zero     => val == 0,
                ConditionValue.Any      => true,
                _                       => throw new ArgumentOutOfRangeException(nameof(comparation), comparation, null)
            };
        }
        
        public static bool Check(this ConditionValue comparation, float val)
        {
            return comparation switch
            {
                ConditionValue.Positive => val > 0,
                ConditionValue.Negative => val < 0,
                ConditionValue.Zero     => val == 0,
                ConditionValue.Any      => true,
                _                       => throw new ArgumentOutOfRangeException(nameof(comparation), comparation, null)
            };
        }
    }
}