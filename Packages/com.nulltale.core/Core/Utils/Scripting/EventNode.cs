using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class EventNode : MonoBehaviour
    {
        public UnityEvent _onInvoke;
        
        // =======================================================================
        public void Invoke()
        {
            _onInvoke.Invoke();
        }
    }
}