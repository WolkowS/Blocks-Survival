using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class ShadowGrainPostProcess : PostProcess.Pass
    {
        private static readonly int s_Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int s_Response  = Shader.PropertyToID("_Response");
        private static readonly int s_Tiling    = Shader.PropertyToID("_Tiling");
        private static readonly int s_GrainTex  = Shader.PropertyToID("_GrainTex");
        private static readonly int s_Tint      = Shader.PropertyToID("_Tint");

        public Texture2D m_Grain;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ShadowGrainSettings>();

            if (settings.IsActive() == false)
                return false;

            var tiling = new Vector4(Screen.width / (float)m_Grain.width, Screen.height / (float)m_Grain.height, Random.value, Random.value);

            mat.SetFloat(s_Intensity, settings.m_Impact.value);
            mat.SetFloat(s_Response, settings.m_Sensitivity.value.OneMinus().Remap(0, 256f));
            mat.SetTexture(s_GrainTex, m_Grain);
            mat.SetVector(s_Tiling, tiling);
            mat.SetVector(s_Tint, settings.m_Tint.value);
            
            return true;
        }
    }
}