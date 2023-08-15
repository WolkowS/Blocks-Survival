using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenLightIntensity : Tween
    {
        private float   m_Impact;
        public  float   m_Scale = 1f;
        private IAdapter  m_Adapter;

        // =======================================================================
        public interface IAdapter
        {
            float Value { get; set; }
        }

        public class Light3D : IAdapter
        {
            public Light _light;
            public float Value { get => _light.intensity; set => _light.intensity = value; }
        }
        
        public class Light2D : IAdapter
        {
            public UnityEngine.Rendering.Universal.Light2D _light;
            public float Value { get => _light.intensity; set => _light.intensity = value; }
        }
        
        // =======================================================================
        private void Awake()
        {
            var root = m_Root.GetValueOrDefault(gameObject);
            
            if (root.TryGetComponent(out Light light3D)) 
                m_Adapter = new Light3D() { _light = light3D };
            else
            if (root.TryGetComponent(out UnityEngine.Rendering.Universal.Light2D light2D))
                m_Adapter = new Light2D() { _light = light2D };
             
        }

        public override void Apply()
        {
            var impact = m_Input.Value * m_Scale;
            if (impact == m_Impact)
                return;

            m_Adapter.Value += (impact - m_Impact);
            m_Impact         =  impact;
        }

        public override void Revert()
        {
            m_Adapter.Value -= m_Impact;
            m_Impact         =  0f;
        }
    }
}