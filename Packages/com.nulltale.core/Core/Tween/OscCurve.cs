using System;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscCurve : TweenOscillator
    {
        public  AnimationCurve m_Value;
        public  float          m_Scale = 1f;
        public  float          m_Interval = 1f;
        private float          m_CurrentTime;

        public override float Value
        {
            get
            {
                m_CurrentTime += DeltaTime;
                return m_Value.Evaluate(m_CurrentTime / m_Interval) * m_Scale;
            }
        }

        public float Interval
        {
            get => m_Interval;
            set => m_Interval = value;
        }
    }
}