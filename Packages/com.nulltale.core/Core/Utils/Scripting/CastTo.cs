using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class CastTo<T> : MonoBehaviour where T : Enum
    {
        public UnityEvent<T> _onInvoke;
        
        // =======================================================================
        public void Invoke(bool val) => Invoke(val.ToInt());
        
        public void Invoke(int val)
        {
            _onInvoke.Invoke((T)(object)val);
        }
    }
}