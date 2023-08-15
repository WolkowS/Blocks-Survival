using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class Signal<T> : IRefGet<T>, IRefSet<T>, IRefGet<Signal<T>>
    {
        [SerializeField]
        private T m_Value;

        public T Value
        {
            get => m_Value;
            set
            {
                if (Equals(m_Value, value))
                    return;
                
                m_Value = value; 
                Notify();
            }
        }

        public event Action<T> React;
        
        public void Notify() => React?.Invoke(m_Value);

        Signal<T> IRefGet<Signal<T>>.Value => this;
        
        public static implicit operator T(Signal<T> s)
        {
            return s.m_Value;
        }
    }
    
    [Serializable]
    public class SignalVoid : IRefGet<SignalVoid>
    {
        public event Action React;
        
        public void Notify() => React?.Invoke();
        
        SignalVoid IRefGet<SignalVoid>.Value => this;
    }
}