using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TinyCounter
    {
        [SerializeField]
        private int m_CurrentTick;
        [SerializeField]
        private int m_FinishTick;
        [SerializeField]
        private bool m_AutoReset = true;
        [SerializeField]
        private bool m_Active = true;

        public bool IsExpired => m_CurrentTick >= m_FinishTick;

        public float Scale => m_CurrentTick / (float)m_FinishTick;

        public bool Active
        {
            get => m_Active;
            set => m_Active = value;
        }

        // =======================================================================
        public bool Tick()
        {
            return Tick(1);
        }

        public bool Tick(int time)
        {
            if (Active)
                m_CurrentTick += time;

            var expired = IsExpired;
            if (m_AutoReset && expired)
                m_CurrentTick -= m_FinishTick;

            return expired;
        }

        public void Reset()
        {
            m_CurrentTick = 0;
        }
    }
}