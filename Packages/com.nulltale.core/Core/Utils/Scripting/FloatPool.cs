using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class FloatPool : MonoBehaviour, IRef<float>
    {
        public Vers<float> _max;
        public Vers<float> _value;
        public bool        _clamped = true;
        public bool        _lock;
        [HideInInspector]
        public Ref<float>  _scale;

        public float Value
        {
            get => _value.Value;
            set
            {
                if(_value.Value == value)
                    return;
                
                _value.Value = value.Clamp(0, _clamped ? _max.Value : int.MaxValue);
                _scale.Value = (_value.Value / _max.Value).Clamp01();
            }
        }

        public float Scale => _scale.Value;

        public float Rest => _max.Value - _value.Value;

        public bool Lock
        {
            get => _lock;
            set => _lock = value;
        }

        // =======================================================================
        public void Restore()
        {
            Value = 0f;
        }
        
        public void Add(GvFloat val) => Add(val.Value);
        
        public void Add(float val)
        {
            if (_lock)
                return;
            
            var result = (_value.Value + val).Clamp(0, _clamped ? _max.Value : int.MaxValue);
            var add    = result - _value.Value;
            _scale.Value = (result / _max.Value).Clamp01();
            _value.Value = result;
        }
    }
}