using UnityEngine;

namespace CoreLib.Values
{
    public class GvVec3Impact : SetGlobalValue<Vector3>
    {
        public  GvVec3  _globalValue;
        public  Vector3 _value;
        private Vector3 _realValue;

        public Vector3 Value
        {
            get => _value;
            set => _apply(value);
        }

        // =======================================================================
        private void OnEnable()
        {
            _apply(_value);
        }

        private void OnDisable()
        {
            _apply(Vector3.zero);
        }

        private void OnValidate()
        {
            _apply(_value);
        }

        private void _apply(Vector3 value)
        {
            if (_realValue == value)
                return;
                
            var impact = value - _realValue;
            _value     = value;
            _realValue = value;
            
            _globalValue.Value += impact;
        }
    }
}