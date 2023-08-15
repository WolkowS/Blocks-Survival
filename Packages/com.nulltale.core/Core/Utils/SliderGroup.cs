using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    [DefaultExecutionOrder(2)] [ExecuteAlways]
    public class SliderGroup : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)]
        private float m_Value = 1.0f;
        private float         m_ValuePrev = -1f;
        private List<ISlider> m_Links     = new List<ISlider>();

        public float Lerp
        {
            get => m_Value;
            set => m_Value = value;
        }

        // =======================================================================
        private void Start()
        {
            Link();
            Update();
        }

        private void Update()
        {
            if (m_Value != m_ValuePrev)
            {
                foreach (var link in m_Links)
                    link.Lerp = m_Value;

                m_ValuePrev = m_Value;
            }
        }

        [Button]
        public void Link()
        {
            m_Links.Clear();
            m_Links.AddRange(GetComponentsInChildren<ISlider>());
        }
    }
}