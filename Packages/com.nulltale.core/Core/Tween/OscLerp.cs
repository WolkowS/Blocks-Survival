using System;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscLerp : TweenOscillator, IToggle
    {
        [Range(0, 1)]
        public float                      m_Pos;
        public float                      m_Value;
        public float                      m_Duration = 1f;
        public Vector2                    m_Range    = new Vector2(0, 1);
        public Optional<AnimationCurve01> m_Lerp;

        public override float Value
        {
            get
            {
                var delta = DeltaTime;
                if (m_Value > m_Pos)
                {
                    m_Value -= delta / m_Duration;
                    if (m_Value < m_Pos)
                        m_Value = m_Pos;
                }
                else
                if (m_Value < m_Pos)
                {
                    m_Value += delta / m_Duration;
                    if (m_Value > m_Pos)
                        m_Value = m_Pos;
                }

                var scale = m_Value;
                if (m_Lerp.Enabled)
                    scale = m_Lerp.Value.Evaluate(scale);
                
                return Mathf.LerpUnclamped(m_Range.x, m_Range.y, scale);
            }
        }

        public float Duration
        {
            get => m_Duration;
            set => m_Duration = value;
        }

        public float Pos
        {
            get => m_Pos;
            set => m_Pos = value;
        }

        public bool IsOn => Pos > 0f;
        public void On() => Pos = 1f;
        public void Off() => Pos = 0f;
    }
}