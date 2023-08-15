using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class EventContainer : MonoBehaviour
    {
        public bool       _enableOnly;
        public UnityEvent _onInvoke;

        // =======================================================================
        public void Invoke()
        {
            if (_enableOnly && gameObject.activeInHierarchy == false)
                return;
            
            _onInvoke.Invoke();
        }
    }
}