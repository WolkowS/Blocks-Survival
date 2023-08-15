using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [DefaultExecutionOrder(10)] [AddComponentMenu("On Disable")]
    public sealed class OnDisableEvent : MonoBehaviour
    {
        public UnityEvent Event;

        // =======================================================================
        private void OnDisable()
        {
            Event.Invoke();
        }
    }
}