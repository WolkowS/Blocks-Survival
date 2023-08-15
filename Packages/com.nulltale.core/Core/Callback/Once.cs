using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [DefaultExecutionOrder(0)]
    public sealed class Once : CallbackBase
    {
        public UnityEvent _onInvoke;

        // =======================================================================
        private void Awake()
        {
            _onInvoke.Invoke();
            Destroy(this);
        }
    }
}