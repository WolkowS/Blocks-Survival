using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnCollision : CallbackBase
    {
        public Optional<SoFilter> m_Filter;
        public Optional<GvGo>     m_GoVal;
        public UnityEvent         m_OnEnter;
        public UnityEvent         m_OnExit;

        // =======================================================================
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (m_GoVal.Enabled && col.rigidbody != null)
                m_GoVal.Value.Value = col.rigidbody.gameObject;
            
            if (m_Filter.Enabled && m_Filter.Value.Check(col.rigidbody.gameObject) == false)
                return;
            
            m_OnEnter.Invoke();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (m_GoVal.Enabled && other.rigidbody != null)
                m_GoVal.Value.Value = other.rigidbody.gameObject;
            
            if (m_Filter.Enabled && m_Filter.Value.Check(other.gameObject) == false)
                return;
            
            m_OnExit.Invoke();
        }
    }
}