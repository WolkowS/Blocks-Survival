using System;

namespace CoreLib
{
    [Serializable]
    public class AnimationCurveFlex
    {
        public AnimationCurve01 _curve;
        public float            _duartion;
        public float            _scale;
        public float            _offset;
        
        public float Evaluate(float time) => _curve.Evaluate(time / _duartion) * _scale + _offset;

        public float Duration()
        {
            return _duartion;
        }
    }
}