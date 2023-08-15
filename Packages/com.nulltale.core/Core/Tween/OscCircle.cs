using UnityEngine;

namespace CoreLib.Tween
{
    public class OscCircle : TweenOscillator
    {
        public  float m_Scale    = 1f;
        public  float m_Interval = 1f;
        private float m_CurrentTime;

        public override float Value
        {
            get
            {
                m_CurrentTime += DeltaTime;
                return Mathf.Cos(m_CurrentTime / m_Interval) * m_Scale;
            }
        }
        
        public override Vector2 Value2
        {
            get
            {
                m_CurrentTime += DeltaTime;
                return new Vector2(Mathf.Cos(((m_CurrentTime % m_Interval) / m_Interval) * m_Scale * Extensions.PI2),
                                   Mathf.Sin(((m_CurrentTime % m_Interval) / m_Interval) * m_Scale * Extensions.PI2));
            }
        }
        
        public override Vector3 Value3 => Value2;

        public float Interval
        {
            get => m_Interval;
            set => m_Interval = value;
        }
    }
}