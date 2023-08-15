using System;
using UnityEngine;

namespace CoreLib
{
    public interface IRef
    {
    }

    public interface IRefGet<out T> : IRef
    {
        T Value { get; }
    }
    
    public interface IRefSet<in T> : IRef
    {
        T Value { set; }
    }
    
    public interface IRef<T> : IRefSet<T>, IRefGet<T>
    {
    }

    [Serializable]
    public class Ref<T> : IRefSet<T>, IRefGet<T>
    {
        [SerializeField]
        private T m_Value;

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        // =======================================================================
        public Ref() { }

        public Ref(T value)
        {
            Value = value;
        }

        public Ref<T> With(T value)
        {
            m_Value = value;
            return this;
        }

        public static implicit operator T(Ref<T> r)
        {
            return r.m_Value;
        }
    }
}