using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.Render
{
    public class OutlinePostProcess : PostProcess.Pass
    {
        private static readonly int s_Thickness    = Shader.PropertyToID("_Thickness");
        private static readonly int s_Sensitive    = Shader.PropertyToID("_Sensitive");
        private static readonly int s_OutlineColor = Shader.PropertyToID("_OutlineColor");

        public Color _default = Color.black;
        
        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<OutlineSettings>();

            if (settings.IsActive() == false)
                return false;

            mat.SetFloat(s_Thickness, settings.m_Thickness.value.Remap(0, .005f));
            mat.SetFloat(s_Sensitive, settings.m_Sensitive.value.Remap(0, 50f));
            
            var col = settings.m_Color.overrideState ? settings.m_Color.value : _default;
            mat.SetColor(s_OutlineColor, col);
            return true;
        }
    }
}