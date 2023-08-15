using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Input : MonoBehaviour
    {
        public UnityEvent<float> _onInvoke;
        
        // =======================================================================
        public void Invoke(float val)
        {
            _onInvoke.Invoke(val);
        }
    }
}