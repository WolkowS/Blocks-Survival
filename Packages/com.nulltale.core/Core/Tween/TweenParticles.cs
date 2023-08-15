using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenParticles : Tween
    {
        private ParticleSystem                m_ParticleSystem;
        private float                         m_Impact;
        private ParticleSystem.EmissionModule m_Emission;
        
        // =======================================================================
        private void Awake()
        {
            m_ParticleSystem = m_Root.GetValueOrDefault(gameObject).GetComponent<ParticleSystem>();
            m_Emission       = m_ParticleSystem.emission; 
        }

        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == m_Impact)
                return;

            m_Emission.rateOverTimeMultiplier += impact - m_Impact;
            m_Impact                           = impact;
        }

        public override void Revert()
        {
            m_Emission.rateOverTimeMultiplier -= m_Impact;
            m_Impact = 0;
        }
    }
}