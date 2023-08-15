using System;
using UnityEngine;

namespace CoreLib
{
    public class MotionRotator : MonoBehaviour
    {
        [SerializeField]
        private int m_TrackLenght = 3;

        public float      m_Speed;
        private Vector3[] m_Track;
        private int       m_TackIndex;
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
            Array.Clear(m_Track, 0, m_Track.Length);
        }

        public void Update()
        {
            _update(Time.deltaTime);
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

            var desired = speedVector.To2DXY().normalized.AngleDeg();
            
            if (speedVector.magnitude < 0.01f)
                desired = 0f;
            else
            if (desired > 90f)
                desired -= 180f;
            else
            if (desired < -90f)
                desired += 180f;

            var current = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, desired, m_Speed * deltaTime);
            transform.localRotation = Quaternion.AngleAxis(current, Vector3.forward);
        }
    }
}