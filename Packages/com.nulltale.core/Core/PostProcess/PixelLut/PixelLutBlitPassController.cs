using CoreLib;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class PixelLutBlitPassController : Blit.BlitPassController
    {
        private static readonly int s_Lut    = Shader.PropertyToID("_Lut");
        private static readonly int s_Weight = Shader.PropertyToID("_Weight");

        private         bool m_IsActive;
        public override bool IsActive => m_IsActive;

        // =======================================================================
        public override void Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<PixelLutSettings>();

            m_IsActive = settings.IsActive();
            if (m_IsActive == false)
                return;

            mat.SetTexture(s_Lut, settings.m_Lut.value);
            mat.SetFloat(s_Weight, settings.m_Contribution.value);
        }
    }
}