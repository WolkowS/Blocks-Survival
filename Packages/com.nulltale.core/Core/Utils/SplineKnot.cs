using UnityEngine;
using UnityEngine.Splines;

namespace CoreLib
{
    [ExecuteAlways]
    public class SplineKnot : MonoBehaviour
    {
        public SplineContainer _spline;
        public int             _index;
        public int             _knot;
        private Vector3        _posLast;
        private Vector3        _scaleLast;
        private Quaternion     _rotLast;
        
        // =======================================================================
        
        private void Update()
        {
            if (_posLast == transform.localPosition && _rotLast == transform.localRotation && _scaleLast == transform.localScale)
                return;
            
            var direction = transform.right * transform.localScale.magnitude;
            _spline[_index].SetKnot(_knot, new BezierKnot(transform.localPosition, -direction, direction, transform.localRotation));
            
            _posLast = transform.localPosition;
            _rotLast = transform.localRotation;
            _scaleLast = transform.localScale;
        }
    }
}