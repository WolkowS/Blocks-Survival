using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.ExposedValues
{
    public class ExposedConditionBool : ExposedCondition
    {
        public  ExposedValueBool m_Source;
        [HideIf(nameof(m_Source))]
        public string     m_SourceID;
        public  Check     m_Check; 

        // =======================================================================
        [Serializable]
        public enum Check
        {
            IsTrue,
            IsFalse
        }

        // =======================================================================
        private void Awake()
        {
            if (m_Source == null)
            {
                var resolver = GetComponentInParent<IResolver>();
                if (resolver != null)
                {
                    m_Source = (ExposedValueBool)resolver.Resolve(m_SourceID);
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
            switch (m_Check)
            {
                case Check.IsTrue:
                    return m_Source.m_Value == true;
                case Check.IsFalse:
                    return m_Source.m_Value == false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}