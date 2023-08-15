using System;
using UnityEngine;
using UnityEngine.Splines;

namespace CoreLib.Play
{
    public class PlayMove : PlayBase
    {
        public Transform        _target;
        public  SplineContainer _spline;
        private SplinePath      _path;
        public  Vector2         _range = new Vector2(0, 1);
        
        // =======================================================================
        private void Awake()
        {
            _path = _spline[0].GetSplinePath();   
        }

        protected override void _onPlay(float scale)
        {
            if (_path.Evaluate(scale, out var pos, out _, out _) == false)
                return;
            
            _target.position = _spline.transform.TransformPoint(pos);
        }

        private void OnValidate()
        {
            if (_spline == null)
                return;
            
            _path = _spline[0].GetSplinePath();
        }
    }
}