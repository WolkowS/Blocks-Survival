using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Tween
{
    public class OscValue : TweenOscillator
    {
        public Vers<float>      m_Value;
        public Vector2          m_Range;
        public AnimationCurve01 m_Curve; 

        // =======================================================================
        public override float Value => m_Range.x + m_Curve.Evaluate(m_Value.Value) * (m_Range.y - m_Range.x);
    }
}