using System;
using UnityEngine;

namespace CoreLib.Values
{
    public class GvFloatImpact : MonoBehaviour
    {
        public  GvFloat _globalValue;
        public  float   _value;
        private float   _realValue;

        public float Value
        {
            get => _value;
            set => _apply(value);
        }

        // =======================================================================
        private void OnEnable()
        {
            _realValue = 0f;
            _apply(_value);
        }

        private void OnDisable()
        {
            _globalValue.Value -= _value;
        }

        private void OnValidate()
        {
            _apply(_value);
        }

        private void _apply(float value)
        {
            if (_realValue == value)
                return;
                
            var impact = value - _realValue;
            _value = value;
            _realValue = value;
            
            _globalValue.Value += impact;
        }
    }
}