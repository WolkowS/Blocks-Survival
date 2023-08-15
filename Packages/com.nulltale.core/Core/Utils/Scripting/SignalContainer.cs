using System;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class SignalContainer<T> : MonoBehaviour, IRef<T>
    {
        public Signal<T> _value;
        public T         Value { get => _value.Value; set => _value.Value = value; }

        private void OnValidate()
        {
            _value.Notify();
        }
    }
}