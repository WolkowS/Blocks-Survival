using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Toggler : MonoBehaviour
    {
        public bool m_IsOn;
        public bool m_ApplyInitial;

        public UnityEvent m_On;
        public UnityEvent m_Off;
        public UnityEvent<bool> m_OnToggle;

        public bool IsOn
        {
            get => m_IsOn;
            set
            {
                if (m_IsOn == value)
                    return;
                
                m_IsOn = value;
                m_OnToggle.Invoke(m_IsOn);
                (m_IsOn ? m_On : m_Off).Invoke();    
            }
        }

        // =======================================================================
        private void Awake()
        {
            if (m_ApplyInitial)
                (m_IsOn ? m_On : m_Off).Invoke();
        }
        
        public void Invoke()
        {
            m_IsOn = !m_IsOn;
            m_OnToggle.Invoke(m_IsOn);
            (m_IsOn ? m_On : m_Off).Invoke();
        }
    }
}