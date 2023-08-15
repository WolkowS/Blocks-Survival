using UnityEngine;
using System;

namespace CoreLib
{
    [Serializable]
    public class TinyTimer
    {
        [SerializeField]
        private float		m_CurrentTime;
        public float TimePassed
        {
            get => m_CurrentTime;
            set => AddTime(value - m_CurrentTime);
        }

        public float Scale      => m_CurrentTime / m_FinishTime;

        [SerializeField]
        private bool        m_AutoReset = true;
        public bool AutoReset
        {
            get => m_AutoReset;
            set
            {
                if (m_AutoReset == value)
                    return;

                m_AutoReset = value;

                // reset by excess of time
                if (m_AutoReset && IsExpired)
                    Reset(Excess, m_FinishTime);
            }
        }

        public bool			IsExpired	=> m_CurrentTime >= m_FinishTime;
        public float		Excess => Mathf.Max(m_CurrentTime - m_FinishTime, 0.0f);
        public float		Remainder => Mathf.Max(m_FinishTime - m_CurrentTime, 0.0f);

        [SerializeField]
        private float		m_FinishTime;
        public float		FinishTime
        {
            get => m_FinishTime;
            set => m_FinishTime = value;
        }

        // =======================================================================
        public bool AddTime(float time)
        {
            var wasExpired = IsExpired;
            m_CurrentTime += time;

            var isExpired = IsExpired;
            if (m_AutoReset && isExpired)
                Reset(Excess, m_FinishTime);

            return wasExpired != isExpired;
        }

        public bool AddDeltaTime()
        {
            return AddTime(Time.deltaTime);
        }

        public bool AddUnscaledDeltaTime()
        {
            return AddTime(Time.deltaTime);
        }

        public bool AddFixedTime()
        {
            return AddTime(Time.fixedDeltaTime);
        }

        /// <summary>
        /// Set current & finish time
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="finishTime"></param>
        public void Reset(float currentTime, float finishTime)
        {
            m_CurrentTime = currentTime;
            m_FinishTime = finishTime;
        }
        /// <summary>
        /// Set finish time & discard current time
        /// </summary>
        /// <param name="finishTime"></param>
        public void Reset(float finishTime)
        {
            m_CurrentTime = 0;
            m_FinishTime = finishTime;
        }
	
        /// <summary>
        /// Set current time to 0.0f
        /// </summary>
        public void Reset()
        {
            m_CurrentTime = 0;
        }

        /// <summary>
        /// Subtract time length from current time, clamp current tine to zero
        /// </summary>
        public void CloseCircle()
        {
            m_CurrentTime = Mathf.Max(m_CurrentTime - m_FinishTime, 0);
        }

        public TinyTimer()
        {
        }

        public TinyTimer(float currentTime, float finishTime, bool autoReset) 
            : this(finishTime, autoReset)
        {
            m_CurrentTime = currentTime;
        }

        public TinyTimer(float finishTime, bool autoReset)
            : this(autoReset)
        {
            m_FinishTime = finishTime;
        }
        
        public TinyTimer(bool autoReset)
        {
            m_AutoReset = autoReset;
        }
    }
}