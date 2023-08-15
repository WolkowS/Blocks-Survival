using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;

namespace CoreLib
{
    public class SplineMove : MonoBehaviour
    {
        public Optional<Vers<GameObject>> _target;
        [Label("Spline")]
        public Optional<SplineContainer> _splineContainer;
        public float _duration;
        [CurveRange]
        public AnimationCurve _lerp;

        private GameObject      _go;
        private SplineContainer _spline;
        private SplinePath      _path;
        private float           _time;

        // =======================================================================
        private void Awake()
        {
            _spline = _splineContainer.Enabled ?  _splineContainer.Value : GetComponent<SplineContainer>();
            _path   = _spline[0].GetSplinePath();
        }

        private void OnEnable()
        {
            _time = 0f;
        }

        private void Update()
        {
            _path.Evaluate(_lerp.Evaluate(_time / _duration), out var pos, out _, out _);
            if (_time == _duration)
            {
                enabled = false;
                return;
            }
            
            transform.position =  pos;
            _time              += Time.deltaTime;
            
            if (_time > _duration)
                _time = _duration;
        }
    }
}