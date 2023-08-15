using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class OnActivation : MonoBehaviour
    {
        public UnityEvent m_OnEnable;
        public UnityEvent m_OnDisable;

        // =======================================================================
        public void Setup(Action onEnable, Action onDisable)
        {
            (m_OnEnable ??= new UnityEvent()).AddListener(onEnable.Invoke);
            (m_OnDisable ??= new UnityEvent()).AddListener(onDisable.Invoke);
        }

        private void OnEnable()
        {
            m_OnEnable?.Invoke();
        }

        private void OnDisable()
        {
            m_OnDisable?.Invoke();
        }
    }
}