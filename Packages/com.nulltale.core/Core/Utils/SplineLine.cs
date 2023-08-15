using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;

namespace CoreLib
{
    [ExecuteAlways]
    public class SplineLine : MonoBehaviour
    {
        [Label("Spline")]
        public Optional<SplineContainer> _splineOverride;
        [Label("Line")]
        public Optional<LineRenderer>    _lineOverride;
        
        private SplineContainer _spline;
        private LineRenderer    _line;

        public Mode  _mode = Mode.Resolution;
        [ShowIf(nameof(_mode), Mode.Points)]
        public int   _points;
        [ShowIf(nameof(_mode), Mode.Resolution)]
        public float _resolution = .3f;
        [MinMaxSlider(0, 1)]
        public Vector2 _range = new Vector2(0, 1);
        public bool _worldTransform = true;
        public bool _editorOnly;

        private Vector2    _rangeLast;
        private float      _resolutionLast;
        private int        _pointsLast;
        private SplinePath _path;
        private float      _pathLenght;
        private bool       _dirty;
        private Matrix4x4  _transformLast;

        // =======================================================================
        public enum Mode
        {
            Points,
            Nodes,
            Resolution
        }
        
        // =======================================================================
        private void Awake()
        {
            _spline = _splineOverride.Enabled ? _splineOverride.Value : GetComponentInParent<SplineContainer>();
            _line   = _lineOverride.Enabled ? _lineOverride.Value : GetComponentInParent<LineRenderer>();
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            Awake();
#endif
            Spline.Changed += _onSplineChanged;
        }

        private void OnDisable()
        {
            Spline.Changed -= _onSplineChanged;
        }

        private void Update()
        {
            if (_editorOnly && Application.isPlaying)
                return;
            
            if (_dirty == false 
                || _range == _rangeLast 
                || _resolution == _resolutionLast
                || _points == _pointsLast
                ||  (_worldTransform && _spline.transform.localToWorldMatrix != _transformLast))
            {
                Build();
            }
        }

        [Button]
        public void Build()
        {
            if (_path == null)
                _getPath();
            
            var spline = _spline[0];

            switch (_mode)
            {
                case Mode.Points:
                {
                    var points = new Vector3[_points];
                    
                    if (_worldTransform)
                    {
                        for (var n = 0; n < _points; n++)
                            points[n] = _spline.transform.TransformPoint(_path.EvaluatePosition((_range.x + (n / (float)(_points - 1)) * _range.y)));
                    }
                    else
                    {
                        for (var n = 0; n < _points; n++)
                            points[n] = _path.EvaluatePosition((_range.x + (n / (float)(_points - 1)) * _range.y));
                    }

                    _line.positionCount = _points;
                    _line.SetPositions(points);
                } break;
                case Mode.Nodes:
                {
                    var knots = spline.Knots.ToList();
                    _line.positionCount = knots.Count();
                    
                    if (_worldTransform)
                    {
                        _line.SetPositions(knots.Select(n => _spline.transform.TransformPoint((Vector3)n.Position)).ToArray());
                    }
                    else
                    {
                        _line.SetPositions(knots.Select(n => (Vector3)n.Position).ToArray());
                    }
                } break;
                case Mode.Resolution:
                {
                    var lenght = _pathLenght * (_range.y - _range.x);
                    var count  = (lenght / _resolution).CeilToInt().Clamp(0, 1000);
                    var points = new Vector3[count];

                    if (_worldTransform)
                    {
                        for (var n = 0; n < count; n++)
                            points[n] = _spline.transform.TransformPoint(_path.EvaluatePosition(_range.x + (n / (float)(count - 1f) * _range.y)));
                    }
                    else
                    {
                        for (var n = 0; n < count; n++)
                            points[n] = _path.EvaluatePosition(_range.x + (n / (float)(count - 1f) * _range.y));
                    }

                    _line.positionCount = count;
                    _line.SetPositions(points);
                    
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _rangeLast      = _range;
            _pointsLast     = _points;
            _resolutionLast = _resolution;
            if (_worldTransform)
                _transformLast = _spline.transform.localToWorldMatrix;
            
            _dirty = false;
        }
        
        // =======================================================================
        private void _onSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (spline != _spline[0])
                return;
         
            _getPath();
        }

        private void _getPath()
        {
            if (_editorOnly && Application.isPlaying)
                return;
            
            _dirty = true;
            _path = _spline[0].GetSplinePath();
            _pathLenght = _path.GetLength();
        }
    }
}