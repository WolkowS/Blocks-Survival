using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class SharpenPostProcess : PostProcess.Pass
    {
        private static readonly int s_Impact = Shader.PropertyToID("_Impact");
        private static readonly int s_Center = Shader.PropertyToID("_Center");
        private static readonly int s_Side   = Shader.PropertyToID("_Side");

        private bool m_IsBox;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<SharpenSettings>();

            if (settings.IsActive() == false)
                return false;

            mat.SetFloat(s_Impact, settings.m_Impact.value);
            
            var steps = mat.IsKeywordEnabled("BOX") ? 8f : 4f;
            
            mat.SetFloat(s_Center, 1f + settings.m_Weight.value * steps);
            mat.SetFloat(s_Side, -settings.m_Weight.value);
            
            return true;
        }
    }
}