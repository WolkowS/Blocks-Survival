using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoreLib.Tween
{
    [DefaultExecutionOrder(100)]
    public abstract class Tween : MonoBehaviour
    {
        public Optional<GameObject> m_Root;
        public UpdateMode           m_Mode;
        
        [Range(0, 1)]
        [SerializeField]
        protected float m_Weight = 1f;
        [FormerlySerializedAs("m_Value")]
        public OscillatorBase m_Input;

        public float Weight
        {
            get => m_Weight;
            set
            {
                m_Weight = value;
                
                if (m_Mode == UpdateMode.Manual)
                    Apply();
            }
        }

        // =======================================================================
        [Serializable]
        public enum UpdateMode
        {
            Update = 0,
            Manual = 3,
        }

        // =======================================================================
        protected virtual void OnDisable()
        {
            Revert();
        }

        private void Update()
        {
            if (m_Mode == UpdateMode.Manual)
                return;
            
            Apply();
        }

        // -----------------------------------------------------------------------
        public abstract void Apply();
        public abstract void Revert();
    }
}