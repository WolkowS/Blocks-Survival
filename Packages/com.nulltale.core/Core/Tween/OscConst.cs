using CoreLib;

namespace CoreLib.Tween
{
    public class OscConst : OscillatorBase
    {
        public          float m_Value;
        public override float Value => m_Value;
        
        public void SetValue(float value)
        {
            m_Value = value;
        }
    }
}