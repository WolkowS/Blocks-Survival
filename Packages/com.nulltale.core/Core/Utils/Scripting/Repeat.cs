using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Repeat : MonoBehaviour
    {
        public Optional<ParticleSystem.MinMaxCurve> m_Interval;
        public ParticleSystem.MinMaxCurve m_Count;
        [ShowIf(nameof(m_Interval))]
        public  bool       m_RealTime;
        public  UnityEvent m_OnInvoke;
        
        private float      m_IntervalValue;
        private float      m_TimePassed;

        // =======================================================================
        public void Invoke()
        {
            var count = m_Count.Evaluate();
            
            if (m_Interval.Enabled)
            {
                StartCoroutine(_run());
            }
            else
            {
                for (var n = 0; n < count; n++)
                    m_OnInvoke.Invoke();
            }
            
            // -----------------------------------------------------------------------
            IEnumerator _run()
            {
                for (var n = 0; n < count; n++)
                {
                    yield return m_RealTime ? new WaitForSecondsRealtime(m_Interval.Value.Evaluate()) : new WaitForSeconds(m_Interval.Value.Evaluate());
                    m_OnInvoke.Invoke();
                }
            }
        }
    }
}