using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnDestroyEvent : MonoBehaviour
    {
        public UnityEvent m_Event;

        // =======================================================================
        private void OnDestroy()
        {
            m_Event.Invoke();
        }
    }
}
