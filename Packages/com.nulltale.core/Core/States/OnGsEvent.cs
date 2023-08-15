using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CoreLib.States
{
    public sealed class OnGsEvent : MonoBehaviour
    {
        [SerializeField]
        private GlobalStateBase m_State;

        [FormerlySerializedAs("m_ApplyInitial")]
        public bool m_OnEnable;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        // =======================================================================
        private void OnEnable()
        {
            m_State.OnOpen  += OnOpen.Invoke;
            m_State.OnClose += OnClose.Invoke;

            if (m_OnEnable)
                (m_State.IsOpen ? OnOpen : OnClose).Invoke();
        }

        private void OnDisable()
        {
            m_State.OnOpen  -= OnOpen.Invoke;
            m_State.OnClose -= OnClose.Invoke;
        }
    }
}