using CoreLib;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace CoreLib.Render
{
    public class ShadowGrainPassController : Blit.BlitPassController
    {
        private static readonly int s_Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int s_Response  = Shader.PropertyToID("_Response");
        private static readonly int s_Tiling    = Shader.PropertyToID("_Tiling");
        private static readonly int s_GrainTex  = Shader.PropertyToID("_GrainTex");

        public Texture2D    m_Grain;

        private         bool m_IsActive;
        public override bool IsActive => m_IsActive;

        // =======================================================================
        public override void Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ShadowGrainSettings>();

            m_IsActive = settings.IsActive();
            if (m_IsActive == false)
                return;

            var tiling  = new Vector4(Screen.width / (float)m_Grain.width, Screen.height / (float)m_Grain.height, Random.value, Random.value);

            mat.SetFloat(s_Intensity, settings.m_Impact.value);
            mat.SetFloat(s_Response, settings.m_Sensitivity.value);
            mat.SetTexture(s_GrainTex, m_Grain);
            mat.SetVector(s_Tiling, tiling);
        }
    }
}