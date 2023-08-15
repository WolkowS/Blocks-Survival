using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnTriggerEnter2DEvent : CallbackBase
    {
        public UnityEvent<Collider2D> m_Event;

        // =======================================================================
        private void OnTriggerEnter2D(Collider2D other)
        {
            m_Event.Invoke(other);
        }
    }
}