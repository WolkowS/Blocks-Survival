using UnityEngine;

namespace CoreLib
{
    public class RandomScale : MonoBehaviour
    {
        [RangeVec2]
        public Vector2 _range;
        public bool    _additive;
        public bool    _onEnable = true;
        public bool    _once = true;

        // =======================================================================
        private void OnEnable()
        {
            if (_onEnable)
                Invoke();
        }

        public void Invoke()
        {
            var value = Random.Range(_range.x, _range.y);
            var roll = new Vector3(value, value, value);
            if (_additive)
                transform.localScale += roll;
            else
                transform.localScale = roll;
            
            if (_once)
                Destroy(this);
        }
    }
}