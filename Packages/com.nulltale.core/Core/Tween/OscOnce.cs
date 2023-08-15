using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Tween
{
    public class OscOnce : TweenOscillator
    {
        public float   m_Scale    = 1;
        public float   m_Delay;
        public float   m_Duration = 1;
        public Vector2 m_Range    = new Vector2(0, 1);
        [CurveRange]
        public AnimationCurve m_Curve = AnimationCurve.Linear(0, 0, 1, 1);
        public bool       m_Destroy;
        public UnityEvent m_OnStart;
        public UnityEvent m_OnComplete;

        private bool  m_IsPlay;
        private bool  m_IsStarted;
        private float m_Time;

        // =======================================================================
        public override float Value
        {
            get
            {
                var duration = m_Duration + m_Delay;
                if (m_Time >= duration)
                {
                    if (m_IsPlay == false)
                    {
                        m_OnComplete.Invoke();
                        if (m_Destroy)
                            Destroy(gameObject);
                    }
                    
                    m_IsPlay = false;
                    m_Time   = duration;
                }
                
                // we must take delta...
                var delta = DeltaTime;
                if (m_IsPlay)
                    m_Time += delta;
                
                if (m_Time <= m_Delay)
                    return m_Range.x * m_Scale;
                
                if (m_IsStarted)
                {
                    m_IsStarted = false;
                    m_OnStart.Invoke();
                }
                
                var lerp = m_Curve.Evaluate((m_Time - m_Delay) / m_Duration);
                return Mathf.LerpUnclamped(m_Range.x, m_Range.y, lerp) * m_Scale;
            }
        }

        private void OnEnable()
        {
            m_IsPlay = true;
            m_IsStarted = true;
            m_Time   = 0f;
            
            _startDeltaTime();
        }
    }
}