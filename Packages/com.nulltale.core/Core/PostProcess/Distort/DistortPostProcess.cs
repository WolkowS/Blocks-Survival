using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class DistortPostProcess : PostProcess.Pass
    {
        private static readonly int   s_Settings = Shader.PropertyToID("_Settings");
        
        private float _offset;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<DistortSettings>();

            if (settings.IsActive() == false)
                return false;

            var sharpness = settings.m_Sharpness.value * 0.1f;
            var tiling    = settings.m_Tiling.value;
            _offset += settings.m_Speed.value * settings.m_Tiling.value * Time.deltaTime;
            
            mat.SetVector(s_Settings, new Vector4(sharpness, tiling, _offset, settings.m_Weight.value));
            
            return true;
        }
    }
}