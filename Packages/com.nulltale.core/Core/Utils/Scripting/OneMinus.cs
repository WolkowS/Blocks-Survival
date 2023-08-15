using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class OneMinus : MonoBehaviour
    {
        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        public void Invoke(float val)
        {
            _onInvoke.Invoke(1f - val);
        }
    }
}