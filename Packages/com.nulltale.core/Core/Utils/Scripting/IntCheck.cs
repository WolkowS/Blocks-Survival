using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class IntCheck : MonoBehaviour
    {
        public Comparison      _input;
        public Vers<int>        _ref;
        public UnityEvent<bool> _onInvoke;
        
        // =======================================================================
        public void Invoke(int val)
        {
            _onInvoke.Invoke(_input.Compare(val, _ref));
        }
    }
}