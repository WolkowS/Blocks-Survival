using System;
using CoreLib.Values;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class FloatOperation : MonoBehaviour
    {
        public Operation         m_Operation;
        public Vers<float>       m_Value;
        
        public UnityEvent<float> m_OnInvoke;

        // =======================================================================
        public void Invoke(float val)
        {
            m_OnInvoke.Invoke(m_Operation.Apply(val, m_Value.Value));
        }
    }
}