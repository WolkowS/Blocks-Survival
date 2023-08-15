using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.States
{
    public sealed class OnGsValue : MonoBehaviour
    {
        [SerializeField]
        private GlobalStateBase m_State;
        public bool             m_Invert;
        public bool             m_OnEnable;
        
        public UnityEvent<bool> OnChanged;

        // =======================================================================
        private void OnEnable()
        {
            m_State.OnOpen  += _on;
            m_State.OnClose += _off;

            if (m_OnEnable)
            {
                if (m_State.IsOpen)
                    _on();
                else
                    _off();
            }
        }

        private void OnDisable()
        {
            m_State.OnOpen  -= _on;
            m_State.OnClose -= _off;
        }
        
        private void _on() => OnChanged.Invoke(!m_Invert);
        private void _off() => OnChanged.Invoke(m_Invert);
    }
}