using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class RandomRotation : MonoBehaviour
    {
        [MinMaxSlider(-360f, 360f)]
        public Vector2 _range;
        public bool    _onEnable = true;

        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            transform.localRotation = Quaternion.Euler(0, 0, Random.Range(_range.x, _range.y));
        }
    }
}