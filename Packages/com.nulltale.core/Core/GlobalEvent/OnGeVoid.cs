using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Events
{
    [ExecuteAlways]
    public class OnGeVoid : MonoBehaviour
    {
        public GeVoid _event;

        public UnityEvent _onInvoke;
        
        // =======================================================================
        public void Invoke()
        {
            _onInvoke.Invoke();
        }
        
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