using System;
using System.Runtime.CompilerServices;

namespace CoreLib
{
    [Serializable]
    public enum TriBool
    {
        False = -1,
        Unknown = 0,
        True = 1,
    }

    public static class TriboolExtentions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AsBool(this TriBool tb)
        {
            return tb == TriBool.True;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TriBool AsTribool(this bool b)
        {
            return b ? TriBool.True : TriBool.False;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(this TriBool tb)
        {
            return tb == TriBool.True;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFalse(this TriBool tb)
        {
            return tb == TriBool.False;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnknown(this TriBool tb)
        {
            return tb == TriBool.Unknown;
        }

        public static bool Equal(this TriBool tb, bool b)
        {
            switch (tb)
            {
                case TriBool.Unknown:
                    return false;
                case TriBool.False:
                    return b == false;
                case TriBool.True:
                    return b == true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tb), tb, null);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TriBool And(this TriBool x, TriBool y)
        {
          if (x.IsTrue() && y.IsTrue())
            return TriBool.True;

          if (x.IsUnknown() || y.IsUnknown())
            return TriBool.Unknown;

          return TriBool.False;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TriBool Or(this TriBool x, TriBool y)
        {
          if (x.IsTrue() || y.IsTrue())
            return TriBool.True;

          if (x.IsUnknown() || y.IsUnknown())
            return TriBool.Unknown;

          return TriBool.False;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TriBool Not(this TriBool x)
        {
            if (x == TriBool.True)
                return TriBool.False;
            
            if (x == TriBool.False)
                return TriBool.True;
            
            return TriBool.Unknown;
        }
    }
}