using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Scripting
{
    public class SpeedTracker : MonoBehaviour
    {
        [SerializeField]
        private float m_Observation = 1;

        private Queue<Sample> m_Track;
        private float         m_Offset;
        private float         m_Time;
        private Vector3       m_LastPositon;

        public UnityEvent<float> _speed;
        
        // =======================================================================
        private struct Sample
        {
            public float _time;
            public float _offset;

            public Sample(float offset, float time)
            {
                _offset = offset;
                _time   = time;
            }
        }
        
        // =======================================================================
        private void Awake()
        {
            m_Track = new Queue<Sample>(60);
        }

        public void OnEnable()
        {
            m_LastPositon = transform.position;
        }

        private void Update()
        {
            _update(Time.deltaTime);
        }

        public void OnDisable()
        {
            Break();
        }

        public void Break()
        {
            m_Track.Clear();
            m_Offset = 0f;
            m_Time   = 0f;
        }

        // =======================================================================
        private void _update(float deltaTime)
        {
            var offset = (transform.position - m_LastPositon).magnitude;
            
            while (m_Time > m_Observation)
            {
                var sample = m_Track.Dequeue();
                m_Offset -= sample._offset;
                m_Time   -= sample._time;
            }
            
            m_Time   += deltaTime;
            m_Offset += offset;
            
            m_Track.Enqueue(new Sample(offset, deltaTime));
            m_LastPositon =  transform.position;
            
            _speed.Invoke(m_Offset / m_Time);
        }
    }
}