using System;
using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenScale : Tween
    {
        private Vector3   m_Impact;
        public  Vector3   m_Scale = Vector3.one;
        private Transform m_Target;

        // =======================================================================
        private void Awake()
        {
            m_Target = m_Root.GetValueOrDefault(gameObject).transform;
        }
        
        public override void Apply()
        {
            var impact = m_Input.Value3.Mul(m_Scale) * Weight;
            if (impact == m_Impact)
                return;

            m_Target.localScale += (impact - m_Impact);
            m_Impact             =  impact;
        }

        public override void Revert()
        {
            m_Target.localScale -= m_Impact;
            m_Impact = Vector3.zero;
        }
    }
}