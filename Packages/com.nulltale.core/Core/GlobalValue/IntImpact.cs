using UnityEngine;

namespace CoreLib.Values
{
    public class IntImpact : MonoBehaviour
    {
        public Vers<int> _value;
        public Vers<int> _impact;

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