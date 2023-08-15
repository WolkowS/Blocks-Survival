using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Values
{
    public class FromValue<TValue> : MonoBehaviour
    {
        public Vers<TValue>       m_Value;
        public bool               m_OnEnable;
        public UnityEvent<TValue> _onInvoke;
        
        // =======================================================================
        private void OnEnable()
        {
            if (m_OnEnable)
                Invoke();
        }

        public void Invoke()
        {
            _onInvoke.Invoke(m_Value.Value);
        }
        
        public void Invoke(TValue val)
        {
            _onInvoke.Invoke(val);
        }
        
        public void InvokeClean()
        {
            m_Value.Resolve();
            Invoke(m_Value.Value);
        }
    }
}