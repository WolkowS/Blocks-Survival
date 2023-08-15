#nullable enable
using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public sealed class Optional<T> : IRefGet<T>, IRefSet<T>
    {
        [SerializeField]
        private bool _enabled;

        [SerializeField]
        private T _value = default!;
    
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public T    Value
        {
            get => _value;
            set => _value = value;
        }

        // =======================================================================
        public Optional()
        {
        }
        
        public Optional(T value)
        {
            _value = value;
        }

        public Optional(T value, bool enabled)
        {
            _enabled = enabled;
            _value   = value;
        }
        
        public T GetValueOrDefault()
        {
#pragma warning disable CS8603
            return _enabled ? _value : default;
#pragma warning restore CS8603
        }

        public T GetValueOrDefault(T fallback)
        {
            return _enabled ? _value : fallback;
        }
        
        public static implicit operator bool(Optional<T> opt)
        {
            return opt._enabled;
        }

        public static implicit operator T(Optional<T> opt)
        {
            return opt._value;
        }
    }
    
    [Serializable]
    public sealed class OptionalRef<TType, TValue> : IRefGet<TValue>, IRefSet<TValue> where TType : IRefGet<TValue>
    {
        [SerializeField]
        internal bool _enabled;

        [SerializeField]
        internal TType _value = default!;
    
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }
        
        public TType Data
        {
            get => _value;
            set => _value = value;
        }

        public TValue Value
        {
            get => _value.Value;
            set => ((IRefSet<TValue>)_value).Value = value;
        }

        // =======================================================================
        public OptionalRef()
        {
        }
        
        public OptionalRef(TType value)
        {
            _value = value;
        }

        public OptionalRef(TType value, bool enabled)
        {
            _enabled = enabled;
            _value   = value;
        }

        public TType GetValueOrDefault()
        {
#pragma warning disable CS8603
            return _enabled ? _value : default;
#pragma warning restore CS8603
        }

        public TType GetValueOrDefault(TType fallback)
        {
            return _enabled ? _value : fallback;
        }
        
        public static implicit operator bool(OptionalRef<TType, TValue> opt)
        {
            return opt._enabled;
        }

        public static implicit operator TType(OptionalRef<TType, TValue> opt)
        {
            return opt._value;
        }
    }
}