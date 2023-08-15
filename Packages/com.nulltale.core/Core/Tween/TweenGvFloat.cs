using CoreLib.Values;

namespace CoreLib.Tween
{
    public class TweenGvFloat : Tween
    {
        public  GvFloat m_GlobalValue;
        private float   m_Impact;

        // =======================================================================
        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == m_Impact)
                return;

            m_GlobalValue.Value += (impact - m_Impact);
            m_Impact            =  impact;
        }

        public override void Revert()
        {
            m_GlobalValue.Value -= m_Impact;
            m_Impact            =  0;
        }
    }
}