using System;
using CoreLib.Scripting;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscPlay : TweenOscillator
    {
        public Mode                       m_Mode     = Mode.Hold;
        public float                      m_Scale    = 1;
        public float                      m_Duration = 1;
        public Vector2                    m_Range    = new Vector2(0, 1);
		public bool                       m_OnEnable = true;
        public Optional<AnimationCurve01> m_Curve; 

        private bool  m_IsPlay;
        private float m_Time;

        // =======================================================================
        public enum Mode
        {
            Stop,
            Hold,
            Loop
        }

        // =======================================================================
        public override float Value
        {
            get
            {
                if (m_Time > m_Duration)
                {
                    switch (m_Mode)
                    {
                        case Mode.Stop:
                        {
                            m_IsPlay = false;
                            m_Time   = 0;
                        } break;
                        case Mode.Hold:
                        {
                            m_IsPlay = false;
                            m_Time   = m_Duration;
                        } break;
                        case Mode.Loop:
                        {
                            while (m_Time >= m_Duration)
                                m_Time -= m_Duration;
                        } break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                var lerp = m_Curve.Enabled ? m_Curve.Value.Evaluate(m_Time / m_Duration) : m_Time / m_Duration;
                
                // we must take delta...
                var delta = DeltaTime;
                if (m_IsPlay)
                    m_Time += delta;
                
                return Mathf.LerpUnclamped(m_Range.x, m_Range.y, lerp) * m_Scale;
            }
        }

		private void OnEnable()
		{
			if (m_OnEnable)
				Play();
            
            _startDeltaTime();
		}

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Play()
        {
            _startDeltaTime();
            
            m_IsPlay = true;
            m_Time   = 0f;
        }
    }
}