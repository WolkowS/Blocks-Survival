using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnTriggerExit2DEvent : CallbackBase
    {
        public UnityEvent<Collider2D> m_Event;

        // =======================================================================
        private void OnTriggerExit2D(Collider2D other)
        {
            m_Event.Invoke(other);
        }
    }
}