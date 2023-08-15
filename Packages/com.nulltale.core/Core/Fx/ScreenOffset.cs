using System;
using CoreLib.Cinemachine;
using UnityEngine;

namespace CoreLib.Fx
{
    [ExecuteAlways]
    public class ScreenOffset : MonoBehaviour
    {
        [Range(0, 1)]
        public float   m_Weight = 1f;
        public Vector3 m_Offset;
        public float   m_Ortho;
        public float   m_Roll;
        
        private Vector3 m_OffsetImpact;
        private float   m_OrthoImpact;
        private float   m_RollImpact;

        public float Weight
        {
            get => m_Weight;
            set => m_Weight = value;
        }

        // =======================================================================
        private void Update()
        {
            CmOffset.s_Impact += m_Offset * m_Weight - m_OffsetImpact;
            CmOrtho.s_Impact  += m_Ortho * m_Weight - m_OrthoImpact;
            CmRoll.s_Impact   += m_Roll * m_Weight - m_RollImpact;
            
            m_OffsetImpact = m_Offset * m_Weight;
            m_OrthoImpact  = m_Ortho * m_Weight;
            m_RollImpact   = m_Roll * m_Weight;
        }

        protected virtual void OnDisable()
        {
            CmOffset.s_Impact -= m_OffsetImpact;
            CmOrtho.s_Impact  -= m_OrthoImpact;
            CmRoll.s_Impact   -= m_RollImpact;
            
            m_OffsetImpact = Vector3.zero;
            m_OrthoImpact  = 0f;
            m_RollImpact   = 0f;
        }
    }
}