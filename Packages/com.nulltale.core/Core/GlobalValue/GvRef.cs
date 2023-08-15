using System;
using UnityEngine;

namespace CoreLib.Values
{
    [Serializable]
    public class GvRef<T>
    {
        [SerializeField]
        internal bool           _useOverride;
        [SerializeField]
        internal GlobalValue<T> _global;
        [SerializeField]
        internal T              _override;
        
        public T Value
        {
            get => _useOverride ? _override : _global.Value;
            set
            {
                if (_useOverride)
                    _override = value;
                else
                    _global.Value = value;
            }
        }
    }
}