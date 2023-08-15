using System;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    public class TransformBehaviour : PlayableBehaviour
    {
        private const float k_RightAngleInRads = Mathf.PI * 0.5f;

        public Transform       startLocation;
        public Transform       endLocation;
        //public SplineContainer spline;
        public bool      position = true;
        public bool      rotation = true;
        public bool      scale    = true;
        public TweenType tween;
        [NonSerialized]
        public TimelineClip clip;
        
        public Vers<AnimationCurve> customCurve = new Vers<AnimationCurve>(AnimationCurve.Linear(0f, 0f, 1f, 1f));

        private AnimationCurve m_LinearCurve       = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        private AnimationCurve m_DecelerationCurve = new AnimationCurve(new Keyframe(0f, 0f, -k_RightAngleInRads, k_RightAngleInRads), new Keyframe(1f, 1f, 0f, 0f));
        private AnimationCurve m_HarmonicCurve     = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        // =======================================================================
        public enum TweenType
        {
            Linear,
            Deceleration,
            Harmonic,
            Custom,
        }

        // =======================================================================
        public float EvaluateCurrentCurve(float time)
        {
            return tween switch
            {
                TweenType.Linear       => m_LinearCurve.Evaluate(time),
                TweenType.Deceleration => m_DecelerationCurve.Evaluate(time),
                TweenType.Harmonic     => m_HarmonicCurve.Evaluate(time),
                TweenType.Custom       => customCurve.Value.Evaluate(time),
                _                      => throw new ArgumentOutOfRangeException()
            };
        }
    }
}