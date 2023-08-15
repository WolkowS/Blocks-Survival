using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class BoolToFloat : MonoBehaviour
    {
        public float _true = 1;
        public float _false;
        
        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        public void Invoke(bool input)
        {
            _onInvoke.Invoke(input ? _true : _false);
        }
    }
}