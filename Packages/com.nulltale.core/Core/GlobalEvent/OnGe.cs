using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Events
{
    public class OnGe<T> : MonoBehaviour
    {
        public GlobalEvent<T> _event;
        public UnityEvent<T>  _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            _event.OnInvoke += _onInvoke.Invoke;
        }

        private void OnDisable()
        {
            _event.OnInvoke -= _onInvoke.Invoke;
        }
    }
}