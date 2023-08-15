using NaughtyAttributes;

namespace CoreLib.Tween
{
    public class OscAdsr : TweenOscillator, IToggle
    {
        public Adsr _adsr;
        [ShowNonSerializedField]
        private int _isOn;
        public bool IsOn => _isOn > 0;
        
        /*public override float Value
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
        }*/
        
        // =======================================================================
        public void On()
        {
            _isOn ++;
        }

        public void Off()
        {
            _isOn --;
        }
    }
}