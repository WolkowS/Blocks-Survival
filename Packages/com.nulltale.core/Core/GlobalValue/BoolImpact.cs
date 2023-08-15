using UnityEngine;

namespace CoreLib.Values
{
    public class BoolImpact : MonoBehaviour
    {
        public Vers<bool> _value;

        // =======================================================================
        private void OnEnable()
        {
            _value.Value = true;
        }

        private void OnDisable()
        {
            _value.Value = false;
        }
    }
}