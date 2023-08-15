using System;
using UnityEngine.Events;

namespace CoreLib
{
    public class ReactFloat : React<float>
    {
        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        protected override void _react(float val)
        {
            _onInvoke.Invoke(val);
        }
    }
}