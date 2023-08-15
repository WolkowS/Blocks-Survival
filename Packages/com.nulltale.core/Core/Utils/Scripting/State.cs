using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class State : MonoBehaviour
    {
        [ReadOnly(inEditor:false)]
        [Label("IsOn")]
        public bool        m_State;

        [ShowNonSerializedField]
        private DirectorState m_Director;

        [ShowNonSerializedField]
        private StateGroup m_Group;
        [Label("On")]
        public UnityEvent  m_OnTrue;
        [Label("Off")]
        public UnityEvent  m_OnFalse;
        
        // =======================================================================
        public void Awake()
        {
            m_Group = transform.parent.GetComponent<StateGroup>();
            m_Director = GetComponent<DirectorState>();
            
            if (m_Director != null)
                m_Director.SetTime(m_State.ToFloat());
            
            _invoke(m_State);
        }
        
        public void OnEnable()
        {
            if (m_State == false)
                _invoke(true);
        }

        public void Toggle() => Invoke(!m_State);
        
        public void On() => Invoke(true);
        public void Off() => Invoke(false);

        public void Invoke(bool value)
        {
            if (m_State == value)
                return;

            _invoke(value);
        }

        // =======================================================================
        private void _invoke(bool value)
        {
            var statePrev = m_State;
            m_State = value;
            
            if (m_Group != null && value)
                m_Group.Invoke(m_Group.transform.GetChildren<State>().ToArray().IndexOf(this));
            
            if (gameObject.activeSelf == false && statePrev == false && value)
                gameObject.SetActive(true);
            
            
            if (gameObject.activeInHierarchy == false)
                return;
            
            StopAllCoroutines();
            StartCoroutine(_apply());
            
            // -----------------------------------------------------------------------
            IEnumerator _apply()
            {
                if (value)
                    m_OnTrue.Invoke();
                else
                    m_OnFalse.Invoke();
                
                if (m_Director != null)
                {
                    m_Director.IsOn = m_State;
                    while (m_Director.IsComplete == false)
                    {
                        yield return null;
                    }
                }
                
                gameObject.SetActive(m_State);
            }
        }
    }
}