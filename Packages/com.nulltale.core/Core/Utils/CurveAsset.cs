using NaughtyAttributes;
using SoCreator;
using UnityEngine;

namespace CoreLib
{
    [SoCreate]
    public class CurveAsset : ScriptableObject
    {
        [SerializeField]
        private AnimationCurve _curve;

        public AnimationCurve Curve => _curve;
        
        public float Evaluate(float time) => _curve.Evaluate(time);
    }
}