using UnityEngine;

namespace CoreLib.Tween
{
    public class OscFlicker : TweenOscillator
    {
        private float                      m_Value;
        private float                      m_TimeLeft;
        public  ParticleSystem.MinMaxCurve m_Interval;
        public  float                      m_Min;
        public  float                      m_Max;
        
        // =======================================================================
        public override float Value
        {
            get
            {
                m_TimeLeft -= DeltaTime;

                if (m_TimeLeft < 0f)
                {
                    m_Value    = Random.Range(m_Min, m_Max);
                    m_TimeLeft = m_Interval.Evaluate();
                }

                return m_Value;
            }
        }
    }
}