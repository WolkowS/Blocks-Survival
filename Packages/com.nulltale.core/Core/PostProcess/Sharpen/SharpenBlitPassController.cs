using CoreLib;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class SharpenBlitPassController : Blit.BlitPassController
    {
        private static readonly int s_Impact = Shader.PropertyToID("_Impact");
        private static readonly int s_Center = Shader.PropertyToID("_Center");
        private static readonly int s_Side   = Shader.PropertyToID("_Side");

        private         bool m_IsActive;
        public override bool IsActive => m_IsActive;
        private bool m_IsBox;

        // =======================================================================
        public override void Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<SharpenSettings>();

            m_IsActive = settings.IsActive();
            if (m_IsActive == false)
                return;

            mat.SetFloat(s_Impact, settings.m_Impact.value);
            
            var steps = mat.IsKeywordEnabled("BOX") ? 8f : 4f;
            
            mat.SetFloat(s_Center, 1f + settings.m_Weight.value * steps);
            mat.SetFloat(s_Side, -settings.m_Weight.value);
        }
    }
}