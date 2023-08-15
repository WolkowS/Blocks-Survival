using System;
using UnityEngine;

namespace CoreLib
{
    public class MotionScaler : MonoBehaviour
    {
        private const float     k_SleepThreshold = 0.0001f;

        [SerializeField]
        private int		                m_TrackLenght = 3;
        [SerializeField]
        private Vector3		            m_ScaleMultiplyer = new Vector3(1.2f, 0.6f, 1.2f);
        [SerializeField]
        private float		            m_SpeedMaxEffect = 0.8f;

        private float                   m_CurrentScale;
        private float                   m_Velocity;
        public  float                   m_Elasticity = 200;
        public  float                   m_Dumping = 10;

        private Vector3[] m_Track;
        private int       m_TackIndex;
        private Vector3   m_Impact;
        private Vector3   m_LastPositon;

        // =======================================================================
        private void Awake()
        {
            m_Track = new Vector3[m_TrackLenght];
        }

        public void OnEnable()
        {
            m_LastPositon = transform.position;
        }

        public void OnDisable()
        {
            Break();
        }

        public void Update()
        {
            _update(Time.deltaTime);
        }

        public void Break()
        {
            // reset inner data
            if (m_Track != null)
                Array.Clear(m_Track, 0, m_TrackLenght);

            m_CurrentScale = 0;
            m_Velocity     = 0;
            
            transform.localScale -= m_Impact;
            m_Impact             =  Vector3.zero;
        }

        // =======================================================================
        private void _update(float deltaTime)
        { 
            // calculate average speed vector
            var speedVector  = Vector3.zero;
            var vectorWeight = 1f / m_TrackLenght;
            foreach (var offset in m_Track)
                speedVector += offset * vectorWeight;
            
            // push new position
            m_Track[m_TackIndex++] = transform.position - m_LastPositon;
            if (m_TackIndex >= m_TrackLenght)
                m_TackIndex = 0;

            m_LastPositon = transform.position;

            // save current speed
            var targetScale = Mathf.Clamp01(speedVector.magnitude / m_SpeedMaxEffect);

            if (Math.Abs(targetScale - m_CurrentScale) < k_SleepThreshold && m_Velocity.Abs() < k_SleepThreshold)
                return;

            var acceleration = (targetScale - m_CurrentScale) * m_Elasticity;
            var dumping      = m_Dumping * m_Velocity;

            m_Velocity += (acceleration - dumping) * deltaTime;

            // calculate relative scale
            m_CurrentScale += m_Velocity * deltaTime;

            var impact = (m_ScaleMultiplyer - Vector3.one) * m_CurrentScale;

            // apply scale
            transform.localScale += impact - m_Impact;
            m_Impact             =  impact;
        }
    }
}