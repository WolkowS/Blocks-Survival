using UnityEngine;

namespace CoreLib.Tween
{
    public class OscPool : TweenOscillator
    {
        public float m_Value;
        public float m_Limit = 1;
        [RangeVec2]
        public Vector2 m_Output = new Vector2(0, 1);
        public float m_Speed = 1;
        public float m_Lerp  = 1;

        // =======================================================================
        public override float Value
        {
            get
            {
                var result = Mathf.Lerp(m_Output.x, m_Output.y, m_Value / m_Limit);
                var delta = DeltaTime;
                
                Add(-(m_Speed * delta + m_Value * m_Lerp * delta));
                
                return result;
            }
        }

        public void Add(float value)
        {
            m_Value = (m_Value + value).Clamp(0, m_Limit);
        }
    }
}