using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenRectSize : Tween
    {
        public  Vector2       m_Scale = Vector2.one;
        private Vector2       m_Impact;
        private RectTransform m_RectTransform;

        // =======================================================================
        private void Awake()
        {
            m_RectTransform = m_Root.GetValueOrDefault(gameObject).GetComponent<RectTransform>();
        }

        public override void Apply()
        {
            var impact = m_Input.Value2.Mul(m_Scale) * Weight;
            if (impact == m_Impact)
                return;

            m_RectTransform.sizeDelta = m_RectTransform.sizeDelta.WithIncXY(impact - m_Impact);
            m_Impact                  = impact;
        }

        public override void Revert()
        {
            m_RectTransform.sizeDelta = m_RectTransform.sizeDelta.WithIncXY(-m_Impact);
            m_Impact                  = Vector2.zero;
        }
    }
}