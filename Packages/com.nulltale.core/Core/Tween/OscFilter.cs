using CoreLib.Values;

namespace CoreLib.Tween
{
    public class OscFilter : OscillatorBase
    {
        public Vers<float>    m_Weight;
        public OscillatorBase m_Input;
        
        public override float Value => m_Input.Value * m_Weight;
    }
}