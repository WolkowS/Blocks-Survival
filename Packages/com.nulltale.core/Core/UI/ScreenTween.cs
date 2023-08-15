using System;
using CoreLib.Steer;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class ScreenTween : MonoBehaviour
    {
        public GvGo m_Anchor;

        public Mode m_Mode = Mode.Speed;
        [ShowIf(nameof(m_Mode), Mode.Speed)]
        public float m_Speed = 1f;
        [ShowIf(nameof(m_Mode), Mode.Time)]
        public float m_Time;

        public LeanTweenType        m_Ease = LeanTweenType.animationCurve;
        [ShowIf(nameof(m_Ease), LeanTweenType.animationCurve)]
        public CurveAsset m_EaseCurve;

        [Vector3Toggle]
        public Vector3 m_Impact = new Vector3(1, 1, 0);

        public UnityEvent m_OnComplete;
        private Vector3 m_ImpactValue;

        // =======================================================================
        [Serializable]
        public enum Mode
        {
            Time,
            Speed
        }

        // =======================================================================
        private void Start()
        {
            var pos = transform.ScreenPosUV();
            // screen pos in uv coords
            var dest = m_Anchor.Value.transform.position;

            if (m_Impact.x == 0f)
                dest.x = pos.x;
            
            if (m_Impact.y == 0f)
                dest.y = pos.y;
            
            if (m_Impact.z == 0f)
                dest.z = pos.z;

            var time = m_Mode switch
            {
                Mode.Speed => Vector3.Distance(pos, dest) / m_Speed,
                Mode.Time  => m_Time,
                _          => throw new ArgumentOutOfRangeException()
            };

            LeanTween.value(gameObject, val =>
                     {
                         var impact = Core.Camera.ScreenToWorldPoint(new Vector3(val.x * Screen.width, val.y * Screen.height, val.z));
                         var offset = impact - m_ImpactValue;
                         m_ImpactValue = impact;

                         transform.position += offset;
                     }, pos, dest, time)
                     .setEase(m_Ease)
                     .setOnComplete(m_OnComplete.Invoke);
        }
    }
}