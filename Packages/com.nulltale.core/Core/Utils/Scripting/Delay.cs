using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class Delay : MonoBehaviour
    {
        [Tooltip("Seconds")]
        public float   m_Delay;
        public bool    m_RealTime;
        public bool    m_Pausable;
        public bool    m_OnEnable;
        
        public UnityEvent m_OnInvoke;

        private Coroutine m_Coroutine;
        private float     m_CompleteTime;
        private bool      m_Pause;

        public float Dealy
        {
            get => m_Delay;
            set => m_Delay = value;
        }

        // =======================================================================
        public void Invoke()
        {
            Invoke(m_Delay);
        }
        
        public void Invoke(float delay)
        {
            m_CompleteTime = (m_RealTime ? Time.unscaledTime : Time.time) + delay;
            if (m_Coroutine == null)
            {
                if (m_Pausable)
                    m_Coroutine = Core.Instance.StartCoroutine(_waitPausable());
                else
                {
                    if (gameObject.activeInHierarchy)
                        m_Coroutine = StartCoroutine(_wait());
                }
            }
        }

        private void OnEnable()
        {
            if (m_OnEnable && m_Coroutine == null)
                Invoke();
            
            m_Pause = false;
        }

        private void OnDisable()
        {
            if (m_Pausable == false)
                m_Coroutine = null;
            
            m_Pause = true;
        }
        
        // =======================================================================
        IEnumerator _wait()
        {
            while (true)
            {
                var finishTime =  m_RealTime ? Time.unscaledTime : Time.time;
                if (m_CompleteTime <= finishTime)
                    break;
                
                yield return null;
            }
            m_Coroutine = null;
            m_OnInvoke.Invoke();
        }
        
        IEnumerator _waitPausable()
        {
            while (true)
            {
                var finishTime =  m_RealTime ? Time.unscaledTime : Time.time;
                
                if (m_Pause)
                    m_CompleteTime += m_RealTime ? Time.unscaledDeltaTime : Time.deltaTime;
                
                if (m_CompleteTime <= finishTime)
                    break;
                
                yield return null;
            }
            
            m_Coroutine = null;
            m_OnInvoke.Invoke();
        }
    }
}