using UnityEngine;

namespace CoreLib.Values
{
    public class FloatImpact : MonoBehaviour
    {
        public Vers<float> _value;
        public Vers<float> _impact;

        // =======================================================================
        private void OnEnable()
        {
            _value.Value += _impact;
        }

        private void OnDisable()
        {
            _value.Value -= _impact;
        }
    }
}