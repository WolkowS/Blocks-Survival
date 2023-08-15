using System;
using CoreLib.Values;
using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenPosition : Tween
    {
        private Vector3   m_Impact;
        public  Vector3   m_Scale = new Vector3(0, 1, 0);
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

            m_Target.localPosition += (impact - m_Impact);
            m_Impact                =  impact;
        }

        public override void Revert()
        {
            m_Target.localPosition -= m_Impact;
            m_Impact                =  Vector3.zero;
        }
    }
}