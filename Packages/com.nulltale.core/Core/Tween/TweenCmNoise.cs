using CoreLib.Module;
using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenCmNoise : MonoBehaviour
    {
        public  float               _amplitude;
        public  float               _frequency;
        public  float               _torque;
        private FxTools.NoiseHandle _nosie;
        
        [Range(0, 1)]
        [SerializeField]
        protected float m_Weight = 1f;

        public float Weight
        {
            get => m_Weight;
            set => m_Weight = value;
        }
        
        // =======================================================================
        protected void OnEnable()
        {
            _nosie = FxTools.Noise();
        }

        protected void OnDisable()
        {
            _nosie.Dispose();
        }

        public void Update()
        {
            _nosie.Amplitude = _amplitude * Weight;
            _nosie.Frequency = _frequency * Weight;
            _nosie.Torque    = _torque * Weight;
        }
    }
}