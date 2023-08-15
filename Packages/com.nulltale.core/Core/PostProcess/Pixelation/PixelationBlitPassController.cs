using CoreLib;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class PixelationBlitPassController : Blit.BlitPassController
    {
        private static readonly int  s_Pixels = Shader.PropertyToID("_Pixels");

        private         bool m_IsActive;
        public override bool IsActive => m_IsActive;

        // =======================================================================
        public override void Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<PixelationSettings>();

            m_IsActive = settings.IsActive();
            if (m_IsActive == false)
                return;

            mat.SetFloat(s_Pixels, settings.m_Scale.value);
        }
    }
}