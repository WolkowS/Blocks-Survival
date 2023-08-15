using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

namespace CoreLib.Scripting
{
    public class OnSignal : MonoBehaviour
    {
        public SignalAsset _key;
        public UnityEvent  _onInvoke;
        
        // =======================================================================
        public void React(SignalAsset key)
        {
            if (_key != key)
                return;
            
            Invoke();
        }
        
        public void Invoke()
        {
            _onInvoke.Invoke();
        }
    }
}