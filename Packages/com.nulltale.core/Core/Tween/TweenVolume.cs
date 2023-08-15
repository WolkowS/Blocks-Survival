using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Tween
{
    public class TweenVolume : Tween
    {
        private Volume m_Volume;

        private float m_Impact;
        
        // =======================================================================
        private void Awake()
        {
            m_Volume = m_Root.GetValueOrDefault(gameObject).GetComponent<Volume>();
        }

        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == m_Impact)
                return;

            m_Volume.weight += impact - m_Impact;
            m_Impact        =  impact;
        }

        public override void Revert()
        {
            m_Volume.weight -= m_Impact;
            m_Impact        =  0;
        }
    }
}