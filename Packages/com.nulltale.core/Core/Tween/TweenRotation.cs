using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenRotation : Tween
    {
        private Vector3   m_Impact;
        public  Vector3   m_Scale = new Vector3(0, 0, 1);
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

            m_Target.localRotation = Quaternion.Euler(m_Target.localRotation.eulerAngles + (impact - m_Impact));
            m_Impact               = impact;
        }

        public override void Revert()
        {
            m_Target.localRotation = Quaternion.Euler(m_Target.localRotation.eulerAngles - m_Impact);
            m_Impact               = Vector3.zero;
        }
    }
}