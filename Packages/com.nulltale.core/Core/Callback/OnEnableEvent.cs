using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [DefaultExecutionOrder(10)] [AddComponentMenu("On Enable")]
    public sealed class OnEnableEvent : CallbackBase
    {
        [Label("On Invoke")]
        public UnityEvent Event;

        // =======================================================================
        private void OnEnable()
        {
            Event.Invoke();
        }
        
        public void Invoke()
        {
            Event.Invoke();
        }
    }
}