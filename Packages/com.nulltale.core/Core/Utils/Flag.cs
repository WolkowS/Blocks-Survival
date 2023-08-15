using System;
using CoreLib.Values;

namespace CoreLib
{
    [Serializable]
    public struct Flag
    {
        public bool _value;
        
        public bool OneShot()
        {
            if (_value)
                return true;
            
            _value = true;
            return false;
        }
        
        public static implicit operator bool(Flag f) => f._value;
    }
}