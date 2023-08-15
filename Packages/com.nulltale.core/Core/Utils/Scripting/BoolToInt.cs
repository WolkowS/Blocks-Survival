using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class BoolToInt : MonoBehaviour
    {
        public int _true = 1;
        public int _false;
        
        public UnityEvent<int> _onInvoke;
        
        // =======================================================================
        public void Invoke(bool input)
        {
            _onInvoke.Invoke(input ? _true : _false);
        }
    }
}