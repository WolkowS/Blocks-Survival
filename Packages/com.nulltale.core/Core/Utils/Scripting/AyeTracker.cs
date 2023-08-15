using System.Linq;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class AyeTracker : MonoBehaviour
    {
        public GameObject     _root;
        public AddLerp        _vel;
        public Vector2        _rect;
        public float          _radius;
        public Vector2        _default;
        public float          _minValue;
        [Tooltip("Used for min value evaluation")]
        public float                _distView;
        [Tooltip("Used for aye partial movement")]
        public float                _distLimit;
        public bool                 _tracked;
        public GlobalList<Weighted> _targets;
        
        // =======================================================================
        private void Update()
        {
            var pos  = _root.transform.position.To2DXY();
            var goal = _default;
            
            if (_tracked)
            {
                var target = _getTarget(); 
                if (target != null)
                {
                    var toGoal = target.transform.position.To2DXY() - pos;
                    var goalDist = toGoal.magnitude;
                    if (goalDist > _minValue)
                    {
                        goal   = toGoal.normalized * ((goalDist / _distLimit).Clamp01() * _radius);
                        goal.x = goal.x.ClampAbs(_rect.x);
                        goal.y = goal.y.ClampAbs(_rect.y);
                    }
                }
            }
            
            transform.localPosition = _vel.Evaluate(transform.localPosition.To2DXY(), goal, Time.deltaTime).To3DXY();
            // -----------------------------------------------------------------------
            Weighted _getTarget()
            {
                Weighted result = null;
                var max = float.MinValue;
                foreach (var weighted in _targets.Value)
                {
                    var value = (Vector2.Distance(pos, weighted.transform.position) / _distView).OneMinus().Clamp01() * weighted._weight;
                    if (value > _minValue && max < value)
                    {
                        result = weighted;
                        max = value;
                    }
                }
                
                return result;
            }
        }
    }
}