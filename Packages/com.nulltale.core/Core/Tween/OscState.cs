using UnityEngine;

namespace CoreLib.Tween
{
    public class OscState : TweenOscillator, IToggle
    {
        public bool            m_IsOn;
        public Optional<float> m_Duration = new Optional<float>(1f);
        public Vector2         m_Range    = new Vector2(0, 1);
        public Optional<AnimationCurve01> _lerp;

        private float m_Scale;

        public override float Value
        {
            get
            {
                if (m_Duration.Enabled)
                {
                    m_Scale += IsOn ? DeltaTime / m_Duration : -(DeltaTime / m_Duration);
                    m_Scale =  m_Scale.Clamp01();
                }
                else
                {
                    m_Scale =  IsOn ? m_Range.y : m_Range.x;
                }
                
                var scale = m_Scale;
                if (_lerp.Enabled)
                    scale = _lerp.Value.Evaluate(scale);
                
                return Mathf.LerpUnclamped(m_Range.x, m_Range.y, scale);
            }
        }

        public bool IsOn
        {
            get => m_IsOn;
            set => m_IsOn = value;
        }

        public float Duration
        {
            get => m_Duration.Value;
            set => m_Duration.Value = value;
        }

        public void On() => IsOn = true;
        public void Off() => IsOn = false;
    }
}