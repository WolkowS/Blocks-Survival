using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Events
{
    public class InvokeGe<T> : MonoBehaviour, InvokeActivator.IHandle
    {
        public GlobalEvent<T> _event;
        public T              _value;
        
        // =======================================================================
        [Button]
        public void Invoke()
        {
            _event.Invoke(_value);
        }
    }
}