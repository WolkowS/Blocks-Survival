using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnTrigger : CallbackBase
    {
        public Optional<SoFilter> m_Filter;
        public Optional<GvGo>     m_GoVal;
        public UnityEvent         m_OnEnter;
        public UnityEvent         m_OnExit;

        // =======================================================================
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_GoVal.Enabled && other.attachedRigidbody != null)
                m_GoVal.Value.Value = other.attachedRigidbody.gameObject;
            
            if (m_Filter.Enabled && m_Filter.Value.Check(other.gameObject) == false)
                return;
            
            m_OnEnter.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (m_GoVal.Enabled && other.attachedRigidbody != null)
                m_GoVal.Value.Value = other.attachedRigidbody.gameObject;
            
            if (m_Filter.Enabled && m_Filter.Value.Check(other.gameObject) == false)
                return;
            
            m_OnExit.Invoke();
        }
    }
}