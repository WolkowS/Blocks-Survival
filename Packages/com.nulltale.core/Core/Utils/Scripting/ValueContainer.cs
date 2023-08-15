using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class ValueContainer<T> : MonoBehaviour, IRef<T>, IRefGet<Signal<T>>
    {
        public  Vers<Signal<T>> _value;
        public  T               Value { get => _value.Value.Value; set => _value.Value.Value = value; }
        
        Signal<T> IRefGet<Signal<T>>.Value => _value.Value;
        
        private void OnValidate()
        {
            _value.Value.Notify();
        }
    }
}