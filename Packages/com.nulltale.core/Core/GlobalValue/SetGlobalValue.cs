using System;
using UnityEngine;

namespace CoreLib.Values
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class SetGlobalValue<TValue> : MonoBehaviour
    {
        public Vers<TValue>        m_Data;
        public GlobalValue<TValue> m_Value;
        
        public bool m_OnEnable;
        
        // =======================================================================
        private void OnEnable()
        {
            if (m_OnEnable)
                Invoke();
        }

        public void Invoke()
        {
            m_Value.Value = m_Data;
        }
    }
}