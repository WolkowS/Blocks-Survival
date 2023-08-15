using UnityEngine;

namespace CoreLib
{
    public class RandomPosition : MonoBehaviour
    {
        public Vector3 _amplitude;
        public bool    _additive;

        // =======================================================================
        private void Awake()
        {
            var roll = new Vector3(_amplitude.x.Amplitude(), _amplitude.y.Amplitude(), _amplitude.z.Amplitude());
            
            if (_additive)
                transform.localPosition += roll;
            else
                transform.localPosition = roll;
        }
    }
}