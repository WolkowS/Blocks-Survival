using UnityEngine;

namespace CoreLib.Tween
{
    public class OscRandom : TweenOscillator
    {
        public ParticleSystem.MinMaxCurve m_Value;
        
        // =======================================================================
        public override float Value => m_Value.Evaluate();
    }
}