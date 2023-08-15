using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class OverDistance : MonoBehaviour
    {
        public  Vers<float>         _interval;
        public bool                 _onEnable;
        public  UnityEvent<Vector3> _onInvoke;
        private Vector3             _anchor;

        // =======================================================================
        private void OnEnable()
        {
            _anchor = transform.position;
            if (_onEnable)
                _onInvoke.Invoke(transform.position);
        }

        private void Update()
        {
            var toVec = transform.position - _anchor; 
            var dist  = toVec.magnitude;
            if (dist > _interval.Value)
            {
                var path = 0f;
                var norm = toVec.normalized;
                var pos  = transform.position;
                while (dist > _interval.Value)
                {
                    path += _interval.Value;
                    dist -= _interval.Value;
                    
                    _onInvoke.Invoke(pos - norm * path);
                }
                
                _anchor = transform.position;
            }
        }
    }
}