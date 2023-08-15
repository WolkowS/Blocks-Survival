using System;

namespace CoreLib
{
    [Serializable]
    public enum ConditionBool
    {
        Any,
        True,
        False,
    }

    public static class ConditionBoolExtentions
    {
        public static bool Check(this ConditionBool condition, bool value)
        {
            switch (condition)
            {
                case ConditionBool.True:
                    return value == true;
                case ConditionBool.False:
                    return value == false;
                case ConditionBool.Any:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition), condition, null);
            }
        }
    }
}