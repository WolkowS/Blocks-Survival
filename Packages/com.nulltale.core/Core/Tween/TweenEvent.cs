using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Tween
{
    public class TweenEvent : Tween
    {
        [SerializeField]
        private UnityEvent<float> m_OnUpdate;

        private float m_Impact;

        // =======================================================================
        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == m_Impact)
                return;

            m_OnUpdate.Invoke(impact);
            m_Impact = impact;
        }

        public override void Revert() { }
    }
}