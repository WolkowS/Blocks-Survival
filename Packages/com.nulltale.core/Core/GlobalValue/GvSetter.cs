using System;
using UnityEngine;

namespace CoreLib.Values
{
    public class GvSetter<TValue, TGlobaValue> : MonoBehaviour where TGlobaValue : class, IGlobalValue<TValue>
    {
        public  TGlobaValue m_GlobalValue;
        public  TValue      m_Value;
        private TValue      m_ValuePrev;

        public bool m_AutoUpdate;

        // =======================================================================
        private void Update()
        {
            if (m_AutoUpdate)
            {
                if (m_ValuePrev.Equals(m_Value) == false)
                {
                    m_GlobalValue.Value = m_Value;
                    m_ValuePrev         = m_Value;
                }
            }
        }
    }
}