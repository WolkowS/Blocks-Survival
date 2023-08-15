using System.Collections;
using UnityEngine;

namespace CoreLib
{
    [RequireComponent(typeof(TimeHandle))]
    public class TimeHandleInvoker : MonoBehaviour
    {
        public  float                    m_Duration = 1f;
        public  Optional<AnimationCurve> m_TimeScale;
        private TimeHandle               m_TimeHandle;
        private float                    m_FinishTime;

        // =======================================================================
        private void Awake()
        {
            m_TimeHandle = GetComponent<TimeHandle>();
        }

        private void OnEnable()
        {
            m_TimeHandle.enabled = false;
        }

        public void Invoke()
        {
            // start coroutine or reset time
            if (m_FinishTime <= Time.unscaledTime)
                StartCoroutine(_waitTimeHandleUnscaled());
            else
                m_FinishTime = Time.unscaledTime + m_Duration;
        }

        public void InvokeTimeEffect()
        {
            Invoke();
        }

        // =======================================================================
        private IEnumerator _waitTimeHandleUnscaled()
        {
            m_TimeHandle.enabled = true;

            m_FinishTime = Time.unscaledTime + m_Duration;

            while (m_FinishTime >= Time.unscaledTime)
            {
                if (m_TimeScale)
                    m_TimeHandle.Scale = m_TimeScale.Value.Evaluate((m_FinishTime - Time.unscaledTime).Ratio01(m_Duration).OneMinus() * m_TimeScale.Value.Duration());

                yield return null;
            }
            
            m_TimeHandle.enabled = false;
        }
    }
}