using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.ExposedValues
{
    public class ExposedConditionInt : ExposedCondition
    {
        public ExposedValueInt m_Source;
        [HideIf(nameof(m_Source))]
        public string   m_SourceID;
        public Check    m_Check;
        [HideIf(nameof(m_RefValue))]
        public int      m_Ref;
        public Optional<ExposedValueInt> m_RefValue;

        // =======================================================================
        [Serializable]
        public enum Check
        {
            Greater,
            Less,
            Equal,

            NotEqual,
            LessOrEqual,
            GreaterOrEqual,
        }

        // =======================================================================
        private void Awake()
        {
            if (m_Source == null)
            {
                var resolver = GetComponentInParent<IResolver>();
                if (resolver != null)
                {
                    m_Source = (ExposedValueInt)resolver.Resolve(m_SourceID);
                }
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (m_Source == null)
                return;

            m_SourceID = m_Source.m_ID;
        }

        public override bool IsMet()
        {
            var refValue = m_RefValue.Enabled ? m_RefValue.Value.Value : m_Ref;
            switch (m_Check)
            {
                case Check.Greater:
                    return m_Source.m_Value > refValue;
                case Check.Less:
                    return m_Source.m_Value < refValue;
                case Check.Equal:
                    return m_Source.m_Value == refValue;
                case Check.NotEqual:
                    return m_Source.m_Value != refValue;
                case Check.LessOrEqual:
                    return m_Source.m_Value <= refValue;
                case Check.GreaterOrEqual:
                    return m_Source.m_Value >= refValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}