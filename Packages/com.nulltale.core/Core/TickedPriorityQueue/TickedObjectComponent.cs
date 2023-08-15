using System;
using UnityEngine;

namespace CoreLib
{
    public class TickedObjectComponent : MonoBehaviour, ITicked
    {
        [SerializeField]
        private TickedQueue m_UpdateQueue;
        [SerializeField]
        private float m_TickLength;

        float ITicked.TickLength => m_TickLength;

        public Action Action;

        // =======================================================================
        void ITicked.OnTicked() => Action?.Invoke();

        private void OnEnable()
        {
            m_UpdateQueue.Add(this);
        }

        private void OnDisable()
        {
            m_UpdateQueue.Remove(this);
        }
    }
}