using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class AnimationCurve01
    {
        public AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        
        public float Evaluate(float time) => _curve.Evaluate(time);
        
        public static implicit operator AnimationCurve(AnimationCurve01 curve) => curve._curve;
    }
}