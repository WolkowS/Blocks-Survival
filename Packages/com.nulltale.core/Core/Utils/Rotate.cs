using UnityEngine;

namespace CoreLib
{
    public class Rotate : MonoBehaviour
    {
        public Vector3 m_Axis = Vector3.back;
        public float   m_DegPerSec;

        // =======================================================================
        private void Update()
        {
            transform.localRotation *= Quaternion.AngleAxis(m_DegPerSec * Time.deltaTime, m_Axis);
        }
    }
}