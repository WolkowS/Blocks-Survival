using CoreLib;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class BlurBlitPassController : Blit.BlitPassController
    {
        private static readonly int s_Directions = Shader.PropertyToID("_Directions");
        private static readonly int s_Quality    = Shader.PropertyToID("_Quality");
        private static readonly int s_Radius     = Shader.PropertyToID("_Radius");

        private         bool m_IsActive;
        public override bool IsActive => m_IsActive;

        // =======================================================================
        public override void Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<BlurSettings>();

            m_IsActive = settings.IsActive();
            if (m_IsActive == false)
                return;

            mat.SetFloat(s_Directions, settings.m_Directions.value);
            mat.SetFloat(s_Quality, settings.m_Steps.value);
            mat.SetFloat(s_Radius, settings.m_Radius.value);
        }
    }
}