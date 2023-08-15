using System.Collections.Generic;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IntEqual : MonoBehaviour
    {
        public bool _invert;
        
        [HideIf(nameof(_set))]
        public Vers<int> _value;
        public Optional<List<int>> _set;

        public UnityEvent _onTrue;
        public UnityEvent<bool> _onInvoke;
        
        // =======================================================================
        public void Invoke(int val)
        {
            var check = val == _value;
            
            if (_set.Enabled)
                check = _set.Value.Contains(val);
            
            if (_invert)
                check = !check;

            if (check)
                _onTrue.Invoke();
            
            _onInvoke.Invoke(check);
        }
    }
}