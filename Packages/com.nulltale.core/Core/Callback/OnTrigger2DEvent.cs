using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnTrigger2DEvent : CallbackBase
    {
        public UnityEvent<bool> m_Event;

        // =======================================================================
        private void OnTriggerEnter2D(Collider2D other)
        {
            m_Event.Invoke(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            m_Event.Invoke(false);
        }
    }
}